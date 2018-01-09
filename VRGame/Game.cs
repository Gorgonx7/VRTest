using SharpDX;
using SharpDX.Windows;
using SharpOVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.Direct3D;

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

        public Game() {
            m_RenderForm = new RenderForm("VRWindow");
            m_RenderForm.ClientSize = new Size(m_Width, m_Height);
            m_RenderForm.AllowUserResizing = false;
        }
        public void Run()
        {
            RenderLoop.Run(m_RenderForm, RenderCallback);
        }

        private void RenderCallback()
        {

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
        }
        private void Draw()
        {
            d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(32, 103, 178));
            swapChain.Present(1, PresentFlags.None);
        }
        public void Dispose()
        {
            m_RenderForm.Dispose();
        }
    }
}
