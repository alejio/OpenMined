﻿using System;
using NUnit.Framework;
using OpenMined.Network.Controllers;
using OpenMined.Syft.NN;

namespace OpenMined.Tests.Editor.FloatTensor
{
    [Category("FloatTensorAutogradTests")]
    public class FloatTensorAutogradTest
    {
        private SyftController ctrl;

        [OneTimeSetUp]
        public void Init()
        {
            //Init runs once before running test cases.
            ctrl = new SyftController(null);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            //CleanUp runs once after all test cases are finished.
        }

        [SetUp]
        public void SetUp()
        {
            //SetUp runs before all test cases
        }

        [TearDown]
        public void TearDown()
        {
            //SetUp runs after all test cases
        }

        /********************/
        /* Tests Start Here */
        /********************/

        [Test]
        public void AddElemAutograd()
        {
            
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});
            var b = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{5,1,3,9,2}, _shape: new int[]{5});

            a.Autograd = true;
            b.Autograd = true;
            
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{6,3,6,13,7}, _shape: new int[]{5});
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            
            var ab_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});

            var c = a.Add(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(ab_grad[i], a.Grad[i]);
                Assert.AreEqual(ab_grad[i], b.Grad[i]);
            }
            
            
            // check that repeating doesn't break it
            c = a.Add(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(ab_grad[i], a.Grad[i]);
                Assert.AreEqual(ab_grad[i], b.Grad[i]);
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Add(b);
            c = a.Add(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(ab_grad[i], a.Grad[i]);
                Assert.AreEqual(ab_grad[i], b.Grad[i]);
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
           
        }
        
        [Test]
        public void AddScalarAutograd()
        {
            
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});

            a.Autograd = true;
            
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{6,7,8,9,10}, _shape: new int[]{5});
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});

            var c = a.Add(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }
            
            
            // check that repeating doesn't break it
            c = a.Add(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Add(5);
            c = a.Add(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
           
        }
        
        [Test]
        public void SubElemAutograd()
        {
            
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});
            var b = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{5,1,3,9,2}, _shape: new int[]{5});

            a.Autograd = true;
            b.Autograd = true;
            
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{-4,1,0,-5,3}, _shape: new int[]{5});
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            var b_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{-1,-1,-1,-1,-1}, _shape: new int[]{5});

            var c = a.Sub(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // subtraction is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
                Assert.AreEqual(b_grad[i], b.Grad[i]);
            }
            
            
            // check that repeating doesn't break it
            c = a.Sub(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
                Assert.AreEqual(b_grad[i], b.Grad[i]);
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Sub(b);
            c = a.Sub(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
                Assert.AreEqual(b_grad[i], b.Grad[i]);
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
              
        }
        
        [Test]
        public void SubScalarAutograd()
        {
            
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});

            a.Autograd = true;
            
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{-4,-3,-2,-1,0}, _shape: new int[]{5});
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});

            var c = a.Sub(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // subtraction is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }
            
            
            // check that repeating doesn't break it
            c = a.Sub(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Sub(5);
            c = a.Sub(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
           
        }
        
        [Test]
        public void DivElemAutograd()
        {
            
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});
            var b = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{5,1,3,8,2}, _shape: new int[]{5});

            a.Autograd = true;
            b.Autograd = true;
            
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{0.2f,2,1,0.5f,2.5f},_shape: new int[]{5});
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{0.2f,1,0.3333f,0.125f,0.5f}, _shape: new int[]{5});
            var b_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{-0.04f,-2,-0.3333f,-0.0625f,-1.25f}, _shape: new int[]{5});

            var c = a.Div(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // division is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.True(Math.Abs(a_grad[i] - a.Grad[i]) < 0.0001);
                Assert.True(Math.Abs(b_grad[i] - b.Grad[i]) < 0.0001);
            }
            
            
            // check that repeating doesn't break it
            c = a.Div(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.True(Math.Abs(a_grad[i] - a.Grad[i]) < 0.0001);
                Assert.True(Math.Abs(b_grad[i] - b.Grad[i]) < 0.0001);
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Div(b);
            c = a.Div(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.True(Math.Abs(a_grad[i] - a.Grad[i]) < 0.0001);
                Assert.True(Math.Abs(b_grad[i] - b.Grad[i]) < 0.0001);
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
              
        }
        
        [Test]
        public void DivScalarAutograd()
        {
            
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});

            a.Autograd = true;
            
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{0.5f,1,1.5f,2,2.5f}, _shape: new int[]{5});
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{.5f,.5f,.5f,.5f,.5f}, _shape: new int[]{5});

            var c = a.Div(2);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // division is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }
            
            
            // check that repeating doesn't break it
            c = a.Div(2);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Div(2);
            c = a.Div(2);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
           
        }
        
        [Test]
        public void MulElemAutograd()
        {
            
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});
            var b = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{5,1,3,8,2}, _shape: new int[]{5});

            a.Autograd = true;
            b.Autograd = true;
            
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{5,2,9,32,10},_shape: new int[]{5});
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{5,1,3,8,2}, _shape: new int[]{5});
            var b_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});

            var c = a.Mul(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // multiplication is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.True(Math.Abs(a_grad[i] - a.Grad[i]) < 0.0001);
                Assert.True(Math.Abs(b_grad[i] - b.Grad[i]) < 0.0001);
            }
            
            
            // check that repeating doesn't break it
            c = a.Mul(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.True(Math.Abs(a_grad[i] - a.Grad[i]) < 0.0001);
                Assert.True(Math.Abs(b_grad[i] - b.Grad[i]) < 0.0001);
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Mul(b);
            c = a.Mul(b);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.True(Math.Abs(a_grad[i] - a.Grad[i]) < 0.0001);
                Assert.True(Math.Abs(b_grad[i] - b.Grad[i]) < 0.0001);
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
              
        }
        
        [Test]
        public void MulScalarAutograd()
        {
            
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,3,4,5}, _shape: new int[]{5});

            a.Autograd = true;
            
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{5,10,15,20,25}, _shape: new int[]{5});
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1}, _shape: new int[]{5});
            
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{5,5,5,5,5}, _shape: new int[]{5});

            var c = a.Mul(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // multiplication is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }
            
            
            // check that repeating doesn't break it
            c = a.Mul(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Mul(5);
            c = a.Mul(5);
            c.Backward(c_grad);

            for (int i = 0; i < a.Size; i++)
            {
                // sum is correct
                Assert.AreEqual(c_expected[i],c[i]);
                
                // gradients are correct
                Assert.AreEqual(a_grad[i], a.Grad[i]);
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
           
        }
        
        [Test]
        public void MMAutograd()
        {

            int[] ash = new int[] {2, 5};
            float[] a_data = new float[] {1, 2, 3, 4, 5, 2, 3, 4, 5, 6};
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: a_data, _shape: ash);

            int[] bs = new int[] {5,3};
            float[] b_data = new float[] {5, 2, 3, 1, 5, 5, 3, 3, 2, 8, 2, 3, 2, 5, 6};
            var b = new Syft.Tensor.FloatTensor(ctrl, _data: b_data, _shape: bs);

            a.Autograd = true;
            b.Autograd = true;

            int[] ces = new int[] {2,3};
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{58,54,61,77,71,80},_shape: ces);
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,1,1,1,1,1}, _shape: new int[]{2,3});

            float[] a_grad_data = new float[] {10, 11, 8, 13, 13, 10, 11, 8, 13, 13};
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: a_grad_data, _shape: ash);

            float[] b_grad_data = new float[] {3, 3, 3, 5, 5, 5, 7, 7, 7, 9, 9, 9, 11, 11, 11};
            var b_grad = new Syft.Tensor.FloatTensor(ctrl, _data: b_grad_data, _shape: bs);

            var c = a.MM(b);
            c.Backward(c_grad);

            for (int i = 0; i < c.Size; i++)
            {
                // multiplication is correct
                Assert.AreEqual(c_expected.Data[i],c.Data[i]);
            }

            for (int i = 0; i < a_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(a_grad.Data[i] - a.Grad.Data[i]) < 0.0001);    
            }
            
            for (int i = 0; i < b_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(b_grad.Data[i] - b.Grad.Data[i]) < 0.0001);    
            }
            
            // check that repeating doesn't break it
            c = a.MM(b);
            c.Backward(c_grad);

            for (int i = 0; i < c.Size; i++)
            {
                // multiplication is correct
                Assert.AreEqual(c_expected.Data[i],c.Data[i]);
            }

            for (int i = 0; i < a_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(a_grad.Data[i] - a.Grad.Data[i]) < 0.0001);    
            }
            
            for (int i = 0; i < b_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(b_grad.Data[i] - b.Grad.Data[i]) < 0.0001);    
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.MM(b);
            c = a.MM(b);
            c.Backward(c_grad);

            for (int i = 0; i < c.Size; i++)
            {
                // multiplication is correct
                Assert.AreEqual(c_expected.Data[i],c.Data[i]);
            }

            for (int i = 0; i < a_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(a_grad.Data[i] - a.Grad.Data[i]) < 0.0001);    
            }
            
            for (int i = 0; i < b_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(b_grad.Data[i] - b.Grad.Data[i]) < 0.0001);    
            }
            
            // cleanup
            ctrl.allow_new_tensors = true;
              
        }
        
        [Test]
        public void TransposeAutograd()
        {

            int[] ash = new int[] {2, 3};
            float[] a_data = new float[] {1, 2, 3, 2, 3, 4};
            var a = new Syft.Tensor.FloatTensor(ctrl, _data: a_data, _shape: ash);

            a.Autograd = true;

            int[] ces = new int[] {3,2};
            var c_expected = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,2,3,3,4},_shape: ces);
            var c_grad = new Syft.Tensor.FloatTensor(ctrl, _data: new float[]{1,2,2,4,3,6}, _shape: ces);

            float[] a_grad_data = new float[] {1,2,3,2,4,6};
            var a_grad = new Syft.Tensor.FloatTensor(ctrl, _data: a_grad_data, _shape: ash);

            var c = a.Transpose();
            c.Backward(c_grad);

            for (int i = 0; i < c.Size; i++)
            {
                // multiplication is correct
                Assert.AreEqual(c_expected.Data[i],c.Data[i]);
            }

            for (int i = 0; i < a_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(a_grad.Data[i] - a.Grad.Data[i]) < 0.0001);    
            }
            
            
            // check that repeating doesn't break it
            c = a.Transpose();
            c.Backward(c_grad);

            for (int i = 0; i < c.Size; i++)
            {
                // multiplication is correct
                Assert.AreEqual(c_expected.Data[i],c.Data[i]);
            }

            for (int i = 0; i < a_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(a_grad.Data[i] - a.Grad.Data[i]) < 0.0001);    
            }

            // see if it's allocating new tensors during the forward pass
            ctrl.allow_new_tensors = false;
            
            // check that repeating the forward pass doesn't break it
            c = a.Transpose();
            c = a.Transpose();
            c.Backward(c_grad);

            for (int i = 0; i < c.Size; i++)
            {
                // multiplication is correct
                Assert.AreEqual(c_expected.Data[i],c.Data[i]);
            }

            for (int i = 0; i < a_grad.Size; i++)
            {
                // a gradients are correct
                Assert.True(Math.Abs(a_grad.Data[i] - a.Grad.Data[i]) < 0.0001);    
            }
            // cleanup
            ctrl.allow_new_tensors = true;
              
        }
        
    }
}