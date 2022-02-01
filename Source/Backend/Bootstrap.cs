using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace ShaderCompilerOnline.Source.Backend
{
	public abstract class BootstrapElement
	{
		public virtual string[] urls { get; }
		public virtual string temporaryDirectory { get; }
	}

	public class DXCBootstrapElement : BootstrapElement
	{
		public override string[] urls
		{
			get
			{
				return new string[]
				{
					"https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.2.0-alpha/dxil.zip",
					"https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.4.1907/dxc_2019-07-15.zip",
					"https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.5.2003/dxc_2020_03-25.zip",
					"https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.5.2010/dxc_2020_10-22.zip",
					"https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.6.2104/dxc_2021_04-20.zip",
					"https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.6.2106/dxc_2021_07_01.zip",
					"https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.6.2112/dxc_2021_12_08.zip"
				};
			}
		}

		public override string temporaryDirectory { get { return "DXC"; } }
	}

	public class Bootstrap
	{
		public Bootstrap()
		{
			var elements = new BootstrapElement[]
			{
				new DXCBootstrapElement(),
			};

			var httpClient = new HttpClient();

			foreach(var element in elements)
			{
				foreach(var url in element.urls)
				{
					var downloadTask = httpClient.GetAsync(url);
					downloadTask.Wait();
					if(!downloadTask.Result.IsSuccessStatusCode)
					{
						continue;
					}
					var byteArrayTask = downloadTask.Result.Content.ReadAsByteArrayAsync();
					byteArrayTask.Wait();
					// byteArrayTask.Result
					System.Diagnostics.Trace(System.IO.Path.)
				}
			}
		}

	}
}
