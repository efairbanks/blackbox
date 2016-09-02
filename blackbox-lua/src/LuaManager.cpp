
#include <assert.h>
#include <string.h>

#include <SDL.h>

#include "LuaManager.hpp"

LuaManager::LuaManager()
{
	mainThread.init();

	mainThread.luaState = luaL_newstate(); // create lua state (context)

	if (mainThread.luaState == NULL)
	{
		LOG_FATAL("Cannot create Lua state: not enough memory.\n");
		exit(-1);
	}

	// setup a hook callback to catch run away scripts
	lua_sethook(mainThread.luaState, &ExecutionCountCallback, LUA_MASKLINE, 0);

	// load all standard lua libraries
	// FIXME: we don't really want to provide all this to users
	luaL_openlibs(mainThread.luaState);

	scriptNextIndex = 0;
	threadNextIndex = 0;
}

LuaManager::~LuaManager()
{
	// allow Lua to free saved references
	for (int i = 0; i < scriptNextIndex; i++)
		luaL_unref(mainThread.luaState, LUA_REGISTRYINDEX, scriptReferenceTable[i]);

	lua_close(mainThread.luaState);
	mainThread.luaState = NULL;
}

/*
 * Traceback function to handle errors from callChunk().
 */
int LuaManager::CallChunkTraceback(lua_State *L)
{
	const char *msg = lua_tostring(L, 1);

	if (msg != NULL)
		luaL_traceback(L, L, msg, 1);
	else if (!lua_isnoneornil(L, 1)) // is there an error object?
	{
		if (!luaL_callmeta(L, 1, "__tostring")) // try its 'tostring' metamethod
			lua_pushliteral(L, "(no error message)");
	}

	return 1;
}

/*
 * Write the Lua version and copyright to the log file.
 */
void LuaManager::logVersion()
{
	LOG_INFO("%s", LUA_COPYRIGHT);
}

/*
 * Log a Lua error which is on the top of the stack then pop it off the stack.
 */
bool LuaManager::logError()
{
	if (lua_isnil(mainThread.luaState, -1)) // nothing to log?
		return false;

	const char *msg = lua_tostring(mainThread.luaState, -1);

	if (msg == NULL)
		msg = "(error object is not a string)";

	// log the error
	LOG_ERROR("Lua: execution ended due to error\n%s", msg);

	// pop error off of stack
	lua_pop(mainThread.luaState, 1);

	// force a complete garbage collection in case of errors
	lua_gc(mainThread.luaState, LUA_GCCOLLECT, 0);

	return true; // error logged
}

/*
 * Call a chunk (may be a function, or a file that was just loaded) that
 * has already been placed on the stack.
 */
int LuaManager::callChunk(int narg, int nres)
{
	int status;
	int base;

	// retieve the chunk index
	base = lua_gettop(mainThread.luaState) - narg;

	// push traceback function to catch errors
	lua_pushcfunction(mainThread.luaState, &CallChunkTraceback);

	// put it under chunk and arguments
	lua_insert(mainThread.luaState, base);

	// set the timing variables to catch run-away chunks
	mainThread.lastTicks = SDL_GetTicks();
	mainThread.tickCounter = 0;

	// set the global lightuserdata for access by our C interface functions
	SetThreadData(mainThread.luaState, &mainThread);

	// call the chunk
	status = lua_pcall(mainThread.luaState, narg, nres, base);

	// remove traceback function after call returns
	lua_remove(mainThread.luaState, base);

	return status;
}

/*
 * Load and run a script file by name.  This is generally used for loading
 * things into the global environment, such as configuration globals etc.  This
 * doesn't allow for the co-routine yield() functionality.
 */
bool LuaManager::runFile(const char *filename)
{
	int status = luaL_loadfile(mainThread.luaState, filename);

	if (status == LUA_OK)
		status = callChunk(0, 0);

	if (status != LUA_OK)
	{
		logError();
		return false;
	}

	return true;
}

/*
 * Loads a Lua script file and saves a reference to it in our table.
 * Does not execute the script.
 */
int LuaManager::loadAndStoreFile(const char* filename)
{
	ASSERT(filename != NULL);
	ASSERT(mainThread.luaState != NULL);

	// FIXME: check for errors here
	luaL_loadfile(mainThread.luaState, filename);

	int fileRef = luaL_ref(mainThread.luaState, LUA_REGISTRYINDEX);

	ASSERT(scriptNextIndex < 100); // FIXME: temp, fixed size

	scriptReferenceTable[scriptNextIndex++] = fileRef;

	return fileRef;
}

/*
 *
 */
int LuaManager::queueStoredChunkSandboxed(int scriptRef)
{
	ASSERT(mainThread.luaState != NULL);

	ASSERT(threadNextIndex < 100); // FIXME: fixed size
	int i = threadNextIndex++;

	// clear the table entry
	threadTable[i].init();

	// create the new thread (will be on top of stack)
	threadTable[i].luaState = lua_newthread(mainThread.luaState);
	threadTable[i].luaRegistryReference = luaL_ref(mainThread.luaState, LUA_REGISTRYINDEX);

	// pointer to save some typing
	lua_State* T = threadTable[i].luaState;

	// get the script from the Lua registry and move it to the new thread's stack
	lua_rawgeti(mainThread.luaState, LUA_REGISTRYINDEX, scriptRef);
	lua_xmove(mainThread.luaState, T, 1);

	lua_newtable(T); // ENV for file 1: S: 21

	// lets have each function have its metatable, where missed lookups are
	// instead looked up in the global table _G

	lua_newtable(T); // metatable S: 54321
	lua_getglobal(T,"_G"); // pushes _G, which will be the __index metatable entry S: 654321

	lua_setfield(T,-2,"__index"); // metatable on top S: 54321
	lua_pushvalue(T,-1); // copy the metatable S: 554321
	lua_setmetatable(T,-3); // set the last copy for env2 S: 54321
	lua_setmetatable(T,-3); // set the original for env1  S: 4321
	// here we end up having 2 tables on the stack for 2 environments
	lua_setupvalue(T,1,1); // first upvalue == _ENV so set it. S: 321
	lua_setupvalue(T,2,1); // set _ENV for file S: 21

	return 0;
}

bool LuaManager::queueFunction(const char* name)
{
	ASSERT(name != NULL);
	ASSERT(mainThread.luaState != NULL);

	// first push function and arguments to make sure all is OK
	lua_getglobal(mainThread.luaState, name);

	if (!lua_isfunction(mainThread.luaState, -1))
	{
		LOG_ERROR("Lua: function '%s' not found\n", name);
		lua_pop(mainThread.luaState, 1);
		return false;
	}

	ASSERT(threadNextIndex < 100); // FIXME: fixed size
	int i = threadNextIndex++;

	threadTable[i].init();

	threadTable[i].done = false;

	threadTable[i].luaState = lua_newthread(mainThread.luaState);
	threadTable[i].luaRegistryReference = luaL_ref(mainThread.luaState, LUA_REGISTRYINDEX);

	// now move the function from the main thread stack to the new thread stack
	// so it is ready to run when processAll() is called
	lua_xmove(mainThread.luaState, threadTable[i].luaState, 1);

	return true;
}

int LuaManager::processAllScripts(float dt)
{
	int status;
	int processedCount = 0;

	for (int i = 0; i < threadNextIndex; i++)
	{
		// skip threads which are done/dead
		if (threadTable[i].done)
			continue;

		threadTable[i].delayCounter -= dt;
		processedCount++;

		// if not enough time has passed don't resume the thread
		if (threadTable[i].delayCounter > 0.0f)
			continue;

		threadTable[i].lastTicks = SDL_GetTicks();
		threadTable[i].tickCounter = 0;

		// Set the global lightuserdata for access by our C interface functions
		// when called from different threads.  This is how we know which thread
		// wants to sleep for however long etc.
		SetThreadData(mainThread.luaState, &threadTable[i]);

		// now resume the chunk
		status = lua_resume(threadTable[i].luaState, NULL, 0);

		// if returned status is yield, no error but chunk yielded so it will
		// be resumed next time
		if (status == LUA_YIELD)
			continue;

		// If we didn't get a yield we either finished successfully or we got
		// an error.  Either way, we mark the thread as done.
		threadTable[i].done = true;

		// if returned stat is OK, we finished the chunk normally, so just continue
		if (status != LUA_OK)
		{
			// if we got here it meant that lua_resume() returned a status other
			// than OK or YIELD, so we log the error

			// grab the error message off the top of the thread stack 
			const char *msg = lua_tostring(threadTable[i].luaState, -1);

			if (msg != NULL)
			{
				// assuming we had an error message on the thread stack,
				// push a stack trace string onto the main Lua stack with the error message
				luaL_traceback(mainThread.luaState, threadTable[i].luaState, msg, 0);
	//			printf("found trackback\n");
			}
			// is there an error object instead of a string?
			else if (!lua_isnoneornil(threadTable[i].luaState, -1))
			{
				// try its 'tostring' metamethod
				if (!luaL_callmeta(threadTable[i].luaState, -1, "__tostring"))
					lua_pushliteral(threadTable[i].luaState, "(no error message)");
			}

			// log the error on the top of the stack and pop it off
			logError();
		}

		// now we are done with this thread
		// unref the thread from the Lua registry to allow the Lua GC to clean it up
		luaL_unref(mainThread.luaState, LUA_REGISTRYINDEX, threadTable[i].luaRegistryReference);
	}

	return processedCount;
}

/*
 * Call a Lua function by name.
 */
bool LuaManager::callFunction(const char* name)
{
	ASSERT(name != NULL);

	// first push function and arguments to make sure all is OK
	lua_getglobal(mainThread.luaState, name);

	if (!lua_isfunction(mainThread.luaState, -1))
	{
		LOG_ERROR("Lua: function '%s' not found\n", name);
		lua_pop(mainThread.luaState, 1);
		return false;
	}

	// now call the chunk in main thread
	callChunk(0, 0);

	return true;
}

template <typename T>
T LuaManager::getGlobalNumber(const char *name)
{
	ASSERT(name != NULL);
	ASSERT(mainThread.luaState != NULL);

	lua_settop(mainThread.luaState, 0);
	lua_getglobal(mainThread.luaState, name);

	if (lua_isnil(mainThread.luaState, 1))
		LOG_WARNING("Lua: global '%s' is not defined", name);
	else if (!lua_isnumber(mainThread.luaState, 1))
		LOG_WARNING("Lua: global '%s' is not a number", name);

	T val = (T) lua_tonumber(mainThread.luaState, 1);

	lua_pop(mainThread.luaState, 1);
	return val;
}

/*
 * Register a single function with Lua.
 */
void LuaManager::registerFunction(const char* name, lua_CFunction func)
{
	ASSERT(name != NULL);

	lua_register(mainThread.luaState, name, func);  
}

/*
 * Register all functions in a given library table with Lua.  This will replace
 * any previously registered global table with the same name
 */
int LuaManager::registerLibrary(const char* name, const luaL_Reg* table)
{
	ASSERT(name != NULL);
	ASSERT(table != NULL);

	lua_newtable(mainThread.luaState);
	luaL_setfuncs(mainThread.luaState, table, 0);
	lua_setglobal(mainThread.luaState, name);

	return 0;
}

void LuaManager::printTable(const char* name)
{
	lua_getglobal(mainThread.luaState, name);
	// stack now contains: -1 => table

	lua_pushnil(mainThread.luaState);
	// stack now contains: -1 => nil; -2 => table

	while (lua_next(mainThread.luaState, -2))
	{
		// stack now contains: -1 => value; -2 => key; -3 => table
		// copy the key so that lua_tostring does not modify the original
		lua_pushvalue(mainThread.luaState, -2);
		// stack now contains: -1 => key; -2 => value; -3 => key; -4 => table
		const char *key = lua_tostring(mainThread.luaState, -1);
		const char *value = lua_tostring(mainThread.luaState, -2);
		printf("%s = '%s'\n", key, value);
		// pop value + copy of key, leaving original key
		lua_pop(mainThread.luaState, 2);
		// stack now contains: -1 => key; -2 => table
	}
 
	// stack now contains: -1 => table (when lua_next returns 0 it pops the key
	// but does not push anything.)
	// Pop table
	lua_pop(mainThread.luaState, 1);

	// Stack is now the same as it was on entry to this function
}

/*
 * Utility functions to set and retrieve our coroutine data from the Lua environment.
 */
void LuaManager::SetThreadData(lua_State* L, LuaManager::ThreadData* threadData)
{
	ASSERT(L != NULL);
	ASSERT(threadData != NULL);

	lua_pushlightuserdata(L, threadData);
	lua_setglobal(L, "LM_T");
}

LuaManager::ThreadData* LuaManager::GetThreadData(lua_State* L)
{
	ASSERT(L != NULL);

	lua_getglobal(L, "LM_T");

//	PrintStack(L);

	if (!lua_islightuserdata(L, -1))
		return NULL;

	LuaManager::ThreadData* data = (LuaManager::ThreadData*) lua_touserdata(L, -1);
	lua_pop(L, 1);

	return data;
}

/*
 * Utility function to print out what is currently on the stack.
 */
int LuaManager::PrintStack(lua_State* L)
{
	int top;

	printf("Values on the stack: \n");

	if ((top = lua_gettop(L)) == 0)
	{
		printf("Stack is empty.\n");
		return 0;
	}

	for (int i = top; i > 0; i--)
	{
		printf("%2d  ", i);

		switch (lua_type(L, i))
		{
		case LUA_TNIL:
			printf("    nil\n");
			break;

		case LUA_TBOOLEAN:
			printf("    boolean: %s\n", (lua_toboolean(L, i) == 0 ? "false" : "true"));
			break;

		case LUA_TLIGHTUSERDATA:
			printf("    lightuserdata\n");
			break;

		case LUA_TNUMBER:
			printf("    number: %f\n", lua_tonumber(L, i));
			break;

		case LUA_TSTRING:
			printf("    string: '%s'\n", lua_tostring(L, i));
			break;

		case LUA_TTABLE:
			printf("    table\n");
			break;

		case LUA_TFUNCTION:
			printf("    function\n");
			break;

		case LUA_TUSERDATA:
			printf("    userdata\n");
			break;

		case LUA_TTHREAD:
			printf("    thread\n");
			break;

		default:
			printf("    unknown\n");
			break;
		}
	}

	return top;
}

/*
 * This callback is installed to keep track of how long chunks run without
 * any yield() calls.  If a chunk is stuck in an infinite loop we catch and
 * kill it this way.
 */
void LuaManager::ExecutionCountCallback(lua_State* L, lua_Debug* ar)
{
    if (ar->event != LUA_HOOKLINE)
		return;

	LuaManager::ThreadData* threadData = LuaManager::GetThreadData(L);

	if (threadData == NULL)
		return;

	int currentTicks = SDL_GetTicks();
	int dt = currentTicks - threadData->lastTicks;
	threadData->lastTicks = currentTicks;

	threadData->tickCounter += dt;

	if (threadData->tickCounter >= 5)
		luaL_error(L, "Maximum execution time without yielding exceeded (5ms)");
}

/*
 * End of LuaManager.cpp
 */

