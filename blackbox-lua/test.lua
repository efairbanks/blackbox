
function do_something_cool()
	-- test the ability to yield from within a lua chunk
	Audio.log("Starting lua test thread a...")
	local i = 1
	while i <= 100 do
		-- print a message once per second
		if (i % 10) == 0 then
			Audio.fireLaser()
			print("FIRIN' LAZERZ!: (a) i="..i)
		end
		-- sleep for 100 ms
		Audio.wait(0.1)
		i = i + 1
	end

	Audio.log("Lua test thread a done.")
end

function do_something_else()
	-- test the ability to yield from within a lua chunk
	Audio.log("Starting lua test thread b...")
	local i = 1
	while i <= 50 do
		if (i % 5) == 0 then
			print("(b) i="..i)
		end
		-- sleep for 200 ms
		Audio.wait(0.2)
		i = i + 1
	end

	Audio.log("Lua test thread b done.")
end


