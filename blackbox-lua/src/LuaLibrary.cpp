
#include <string.h>
#include <stdarg.h>

#include <SDL.h>

#include "LuaManager.hpp"
#include "LuaLibrary.hpp"

#include "BlackBox.hpp"

extern Laser* laser;

/*
 * Table of C functions to register with Lua.
 */
const luaL_Reg LuaLib::LibAudio[] =
{
	{ "log",                   &LuaLib::log                  },
	{ "wait",                  &LuaLib::wait                 },
	{ "fireLaser",		   &LuaLib::fireLaser		 },	

	// NOTE: add your functions to this table

	{ NULL,                    NULL                          }
};

/*
 * Continuation callback when a running chunk yields.
 * FIXME: do something better here.
 */
static int cont(lua_State *L)
{
//	printf("<in cont>\n");
//    getchar();
    return 0;
}

int LuaLib::fireLaser(lua_State* L)
{
	laser->Fire();
	return 0;
}

int LuaLib::log(lua_State* L)
{
	luaL_argcheck(L, (lua_gettop(L) >= 1), 1, "too few arguments");
	luaL_argcheck(L, (lua_gettop(L) <= 1), 2, "too many arguments");
	const char* msg = luaL_checkstring(L, 1);
	luaL_argcheck(L, msg != NULL, 1, "`string' expected");
	LOG_INFO("Lua: %s", msg);
	return 0;
}

int LuaLib::wait(lua_State* L)
{
	// grab the data for the thread that called us
	LuaManager::ThreadData* threadData = LuaManager::GetThreadData(L);
	ASSERT(threadData != NULL);
	ASSERT(L == threadData->luaState);

	luaL_argcheck(L, (lua_gettop(L) >= 1), 1, "too few arguments");
	luaL_argcheck(L, (lua_gettop(L) <= 1), 2, "too many arguments");
	float delay = (float) luaL_checknumber(L, 1);
	luaL_argcheck(L, delay >= 0.001f && delay <= 60.0f*60.0f, 1,
		"value is out-of-range [0.001, 3600]");

	threadData->delayCounter = delay;

	return lua_yieldk(L, 0, 0, cont);
}

/*
 * End of LuaLibrary.cpp
 */
