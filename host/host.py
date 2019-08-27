#!/usr/bin/env python
# host.py

import pyopencl as cl
import numpy as np

def np_aligned_empty(shape, dtype, alignbytes):
    # https://stackoverflow.com/questions/9895787/memory-alignment-for-fast-fft-in-python-using-shared-arrays
    dtype = np.dtype(dtype)
    nbytes = np.prod(shape) * dtype.itemsize
    buf = np.empty(nbytes+alignbytes, dtype=np.uint8)
    start_idx = -buf.ctypes.data % alignbytes
    return buf[start_idx:start_idx + nbytes].view(dtype).reshape(shape)

with open("bin/example1.aocx", "rb") as fid:
    cl_binary = fid.read()

ctx = cl.create_some_context()
queue = cl.CommandQueue(ctx)

mf = cl.mem_flags

x = np.asarray([1, 1<<16])
n = len(x)

x_c = np_aligned_empty((n,), np.uint32, 64); x_c[()] = x[()]
y_c = np_aligned_empty((n,), np.uint32, 64); y_c[()] = 0

x_g = cl.Buffer(ctx, mf.COPY_HOST_PTR, hostbuf=x_c)
y_g = cl.Buffer(ctx, mf.COPY_HOST_PTR, hostbuf=y_c)

prg = cl.Program(ctx, ctx.devices, [cl_binary,])

queue = cl.CommandQueue(ctx)
krn = prg.test_lib
#krn = prg.test_builtin
krn.set_args(x_g, y_g, np.int32(n))
cl.enqueue_nd_range_kernel(queue, krn, (1,1,1), (1,1,1)).wait()

cl.enqueue_copy(queue, y_c, y_g)

print("x: ", x_c)
print("y: ", y_c)

# compare with numpy
y_np = x_c >> 16 | x_c << 16;

print("diff: ", y_c - y_np)
print("np.linalg.norm: ", np.linalg.norm(y_c - y_np))
#assert np.allclose(y_c, y_np)

# ------------------------------------------------------------------------
# Usage: CL_CONTEXT_EMULATOR_DEVICE_INTELFPGA=1 PYOPENCL_CTX='0' python host/host.py
