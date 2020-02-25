
using SharpDX;

using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.D3DCompiler;


using SharpOVR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.Windows;

namespace VRGame
{
    public class Game : IDisposable
    {
        private readonly HMD _hmd;
        private RenderForm m_RenderForm;
        private const int m_Width = 1280;
        private const int m_Height = 720;
        private D3D11.Device d3dDevice;
        private D3D11.DeviceContext d3dDeviceContext;
        private SwapChain swapChain;
        private D3D11.RenderTargetView renderTargetView;
        private Vector3[] vertices = new Vector3[] { new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(0.5f, 0.5f, 0.0f), new Vector3(0.0f, -0.5f, 0.0f) };
        private D3D11.Buffer triangleVertexBuffer;
        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;
        private Viewport viewport;
        private D3D11.InputElement[] inputElements = new D3D11.InputElement[]
        {
            new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0)
        };
        private ShaderSignature inputSignature;
        private D3D11.InputLayout inputLayout;
        public Game() {
            m_RenderForm = new RenderForm("VRWindow");
            m_RenderForm.ClientSize = new Size(m_Width, m_Height);
            m_RenderForm.AllowUserResizing = false;
            InitializeDeviceResources();
            InitializeShaders();
            InitializeTriangle();
        }
        public void Run()
        {
            RenderLoop.Run(m_RenderForm, RenderCallback);
        }

        private void RenderCallback()
        {
            Draw();
        }
        private void InitializeDeviceResources()
        {
            ModeDescription backBufferDesc = new ModeDescription(m_Width, m_Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
            SwapChainDescription swapChainDesc = new SwapChainDescription()
            {
                ModeDescription = backBufferDesc,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = m_RenderForm.Handle,
                IsWindowed = true
            };
            D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.None, swapChainDesc, out d3dDevice, out swapChain);
            d3dDeviceContext = d3dDevice.ImmediateContext;
            using (D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0))
            {
                renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
            }
            // Set viewport
            viewport = new Viewport(0, 0, m_Width, m_Height);
            d3dDeviceContext.Rasterizer.SetViewport(viewport);
        }
        private void InitializeTriangle()
        {
            triangleVertexBuffer = D3D11.Buffer.Create<Vector3>(d3dDevice, D3D11.BindFlags.VertexBuffer, vertices);
        }
        private void InitializeShaders()
        {
            using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile("vertexShader.hlsl", "main", "vs_4_0", ShaderFlags.Debug))
            {
                inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
                vertexShader = new D3D11.VertexShader(d3dDevice, vertexShaderByteCode);
            }
            using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile("pixelShader.hlsl", "main", "ps_4_0", ShaderFlags.Debug))
            {
                pixelShader = new D3D11.PixelShader(d3dDevice, pixelShaderByteCode);
            }
            d3dDeviceContext.VertexShader.Set(vertexShader);
            d3dDeviceContext.PixelShader.Set(pixelShader);
            d3dDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            inputLayout = new D3D11.InputLayout(d3dDevice, inputSignature, inputElements);
            d3dDeviceContext.InputAssembler.InputLayout = inputLayout;
        }
        private void Draw()
        {
            d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(32, 103, 178));
            d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(triangleVertexBuffer, Utilities.SizeOf<Vector3>(), 0));
            d3dDeviceContext.Draw(vertices.Count(), 0);
            swapChain.Present(1, PresentFlags.None);
        }
        public void Dispose()
        {
            inputLayout.Dispose();
            inputSignature.Dispose();
            triangleVertexBuffer.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
            renderTargetView.Dispose();
            swapChain.Dispose();
            d3dDevice.Dispose();
            d3dDeviceContext.Dispose();
            m_RenderForm.Dispose();
        }
    }
}
