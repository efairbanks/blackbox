cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

# project name
project("Black Box Audio Library")

# set compiler warning level
if ("${CMAKE_CXX_COMPILER_ID}" STREQUAL "GNU" OR
    "${CMAKE_CXX_COMPILER_ID}" STREQUAL "Clang")
    set(warnings "-Wall -Werror")
elseif ("${CMAKE_CXX_COMPILER_ID}" STREQUAL "MSVC")
    set(warnings "/W4 /WX /EHsc")
endif()

set(CMAKE_CXX_FLAGS "${warnings}")
set(CMAKE_C_FLAGS   "${warnings}")

# define our source files
set(SOURCES
	main.cpp
	BlackBox.cpp
	LuaManager.cpp
	LuaLibrary.cpp)

# use PkgConfig to find discover SDL2 libraries
include(FindPkgConfig)
pkg_search_module(SDL2 REQUIRED sdl2)
pkg_search_module(SDL2_mixer REQUIRED SDL2_mixer)

if (NOT SDL2_FOUND)
    message(FATAL_ERROR "SDL2 not found!")
endif (NOT SDL2_FOUND)

if (NOT SDL2_mixer_FOUND)
    message(FATAL_ERROR "SDL2_mixer not found!")
endif (NOT SDL2_mixer_FOUND)

set(LIBS ${LIBS}
	${SDL2_LIBRARIES}
	SDL2_mixer)

add_subdirectory(lua-5.2.3)

include_directories(
	${SDL2_INCLUDE_DIRS}
	${PROJECT_BINARY_DIR}
	${PROJECT_SOURCE_DIR}
	${PROJECT_SOURCE_DIR}/lua-5.2.3)

link_directories(
	${PROJECT_BINARY_DIR}/lua-5.2.3)

link_libraries(${LIBS})

add_executable(bbal ${SOURCES})

target_link_libraries(bbal
	lua-5.2.3)

