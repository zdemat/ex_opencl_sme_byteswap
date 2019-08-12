# ex_opencl_sme_byteswap
Example of SME generated code with Altera/Intel OpenCL

## Compiling for emulator

```bash
make em
make host
```

You will need Altera/Intel "common examples code" one folder below this project. Alterantively you can use PyOpenCL host.

Run host application as:

```bash
CL_CONTEXT_EMULATOR_DEVICE_INTELFPGA=1 ./bin/host
# or with python
CL_CONTEXT_EMULATOR_DEVICE_INTELFPGA=1 PYOPENCL_CTX='0' python ./host/host.py
```

## Compiling for hardware


```bash
make hw
```
