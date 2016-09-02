/*
 * This module contains the C functions exposed to the Lua environment.
 */

#pragma once

#include "lua-5.2.3/lua.hpp"
#include "BlackBox.hpp"

namespace LuaLib
{
	extern const luaL_Reg LibAudio[];

	// these are the exposed functions
	int fireLaser(lua_State* L);
	int log(lua_State* L);
	int wait(lua_State* L);
}

