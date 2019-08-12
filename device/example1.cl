// Copyright (C) 2013-2018 Altera Corporation, San Jose, California, USA. All rights reserved.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to
// whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// This agreement shall be governed in all respects by the laws of the State of California and
// by the laws of the United States of America.

// This file is a variation on a code snippet from Altera/Intel. It is a part of small
// project demostrationg how to use Altera/Intel OpenCL with SME generated code.

// Before compiling this kernel, create opencl_lib.aoclib with:
//	  aocl library hdl-comp-pkg device/lib_sme/opencl_lib.xml -o device/lib_sem/opencl_lib.aoco
//	  aocl library create -name device/opencl_lib device/lib_sme/opencl_lib.aoco
// Then compile this kernel with:
//    aoc -o bin/example1.aocx -l device/opencl_lib.aoclib -L./device/lib_sme -I./device/lib_sme -board=p385a_mac_ax115 -v device/example1.cl

unsigned int sme_byteswap(unsigned int x);

// Using HDL library components
kernel void test_lib (global unsigned int * restrict in, global unsigned int * restrict out, int N) {
  int i = get_global_id(0);
  for (int k =0; k < N; k++) {
    unsigned int x = in[i*N + k];
    out[i*N + k] = sme_byteswap(x);
  }
}


// Using identical (in function and implementation) built-in components
kernel void test_builtin (global unsigned int * restrict in, global unsigned int * restrict out, int N) {
  int i = get_global_id(0);
  for (int k = 0; k < N; k++) {
    unsigned int x = in[i*N + k];
    out[i*N + k] = x << 16 | x >> 16;
  }
}
