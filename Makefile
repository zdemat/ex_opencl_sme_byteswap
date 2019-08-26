dir-target:
	mkdir -p device
	mkdir -p bin
	mkdir -p device/lib_sme/vhd

sme-target: device/sme/BusDefinitions.cs device/sme/ByteSwap.csproj device/sme/Processes.cs device/sme/Program.cs device/sme/Simulators.cs
	make dir-target
	cd device/sme/ && dotnet run
	cp device/sme/output/vhdl/ByteSwapProc.vhdl device/lib_sme/vhd/ByteSwapProc.vhd
	cp device/sme/output/vhdl/ByteSwap.vhdl device/lib_sme/vhd/ByteSwap.vhd
	cp device/sme/output/vhdl/Export_ByteSwap.vhdl device/lib_sme/vhd/Export_ByteSwap.vhd
	cp device/sme/output/vhdl/system_types.vhdl device/lib_sme/vhd/system_types.vhd
	cp device/sme/output/vhdl/Types_ByteSwap.vhdl device/lib_sme/vhd/Types_ByteSwap.vhd
	sed -i "s/RST = '1'/RST = '0'/g" device/lib_sme/vhd/*.vhd 
	sed -i "s/RST/resetn/g" device/lib_sme/vhd/*.vhd
	sed -i "s/CLK/clock/g" device/lib_sme/vhd/*.vhd 

lib-target:
	make dir-target
	aocl library hdl-comp-pkg device/lib_sme/opencl_lib.xml -o device/lib_sme/opencl_lib.aoco
	aocl library create -name device/opencl_lib device/lib_sme/opencl_lib.aoco

hw:
	make dir-target
	make sme-target
	make lib-target
	aoc -o bin/example1.aocx -l device/opencl_lib.aoclib -L./device/lib_sme -I./device/lib_sme -board=p385a_mac_ax115 -v device/example1.cl

em:
	make dir-target
	make lib-target
	aoc -march=emulator -o bin/example1.aocx -l device/opencl_lib.aoclib -L./device/lib_sme -I./device/lib_sme -board=p385a_mac_ax115 -v device/example1.cl

host: host/main.cpp
	make dir-target
	g++ -o bin/host -I./../common/inc -lOpenCL ./../common/src/AOCLUtils/opencl.cpp host/main.cpp	
