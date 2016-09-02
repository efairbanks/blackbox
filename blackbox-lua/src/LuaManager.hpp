
#pragma once

#include <assert.h>

// temp: real engine provides much better assert and logging macros
#define ASSERT assert
#define LOG_INFO(...) do { printf(__VA_ARGS__); putchar('\n'); } while (0)
#define LOG_WARNING(...) do { printf(__VA_ARGS__); putchar('\n'); } while (0)
#define LOG_ERROR(...) do { printf(__VA_ARGS__); putchar('\n'); } while (0)
#define LOG_FATAL(...) do { printf(__VA_ARGS__); putchar('\n'); } while (0)

#include "lua-5.2.3/lua.hpp"

class LuaManager
{
public:
	struct ThreadData
	{
		lua_State* luaState;

		float delayCounter;
		int luaRegistryReference;

		int lastTicks;
		int tickCounter;

		bool done;

		// used to clear the thread data when queuing up a new Lua thread
		void init()
		{
			luaState = NULL;

			delayCounter = 0.0f;
			luaRegistryReference = 0;

			lastTicks = 0;
			tickCounter = 0;

			done = true;
		}
	};

private:
	ThreadData mainThread;

	// table of saved script file references
	int scriptReferenceTable[100]; // FIXME: fixed size
	int scriptNextIndex;

	// table of saved running threads
	ThreadData threadTable[100]; // FIXME: fixed size
	int threadNextIndex;

public:
	LuaManager();
	~LuaManager();

	void logVersion();

	bool runFile(const char* filename);

	// NOTE: these three methods are just for testing
	int loadAndStoreFile(const char* filename);
	int queueStoredChunkSandboxed(int ref);
	bool queueFunction(const char* name);

	int processAllScripts(float dt);

	bool callFunction(const char* name); // FIXME: lets take variable arguments

	template <typename T>
	T getGlobalNumber(const char *name);

	void registerFunction(const char *name, lua_CFunction func);
	int registerLibrary(const char* name, const luaL_Reg* table);

	void printTable(const char* name);

private:
	bool logError();
	int callChunk(int narg, int nres);

public:
	static void SetThreadData(lua_State* L, ThreadData* threadData);
	static ThreadData* GetThreadData(lua_State* L);

	static int PrintStack(lua_State* L);

	static void ExecutionCountCallback(lua_State* L, lua_Debug* ar);

	static int CallChunkTraceback(lua_State* L);
};

