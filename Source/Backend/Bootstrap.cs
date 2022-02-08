using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.IO.Compression;

namespace ShaderCompilerOnline.Source.Backend
{
	public abstract class BootstrapElement
	{
		public virtual string[] urls { get; }
		public virtual string workingDirectory { get; }
		public virtual string OnGetSaveFileNameEach(string url) { return url; }
		public virtual void OnExtractEach(string srcURL, string srcPath, string outDir) { }
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

		public override string workingDirectory { get { return "DXC"; } }

		public override void OnExtractEach(string srcURL, string srcPath, string outDir)
		{
			using (var archive = ZipFile.OpenRead(srcPath))
			{
				foreach (var entry in archive.Entries)
				{
					var path = entry.FullName.Replace("\\", "/").ToLower();
					var fileName = Path.GetFileName(path);
					var dir = Path.GetDirectoryName(path);
					var extension = Path.GetExtension(path);
					var isValidExtension = (extension == ".exe" || extension == ".dll");
					if ((dir.EndsWith("x64") || (!dir.EndsWith("x64") && !dir.EndsWith("x86"))) && isValidExtension)
					{
						var relativeDir = Path.GetDirectoryName(srcURL.Remove(0, "https://github.com/microsoft/DirectXShaderCompiler/releases/download/".Length));
						var absoluteDir = Path.Combine(outDir, relativeDir);
						if(!Directory.Exists(absoluteDir))
						{
							Directory.CreateDirectory(absoluteDir);
						}
						entry.ExtractToFile(Path.Combine(absoluteDir, fileName));
					}
				}
			}
		}
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
					var filePath = Path.Combine(GetTemporaryDirectory(element.workingDirectory), Path.GetFileName(url));
					if(File.Exists(filePath))
					{
						continue;
					}
					var downloadTask = httpClient.GetAsync(url);
					downloadTask.Wait();
					if(!downloadTask.Result.IsSuccessStatusCode)
					{
						continue;
					}
					var byteArrayTask = downloadTask.Result.Content.ReadAsByteArrayAsync();
					byteArrayTask.Wait();
					File.WriteAllBytes(filePath, byteArrayTask.Result);
					element.OnExtractEach(url, filePath, GetOutputDirectory(element.workingDirectory));
		//			System.Diagnostics.Trace.WriteLine(GetOutputDirectory(element.temporaryDirectory), fileName);
				}
			}
		}

		private string GetTemporaryDirectory(string subDir)
		{
			var dir = Path.Combine(Directory.GetCurrentDirectory(), "Temp", subDir);
			if(!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			return dir;
		}

		private string GetOutputDirectory(string subDir)
		{
			var dir = Path.Combine(Directory.GetCurrentDirectory(), "External", subDir);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			return dir;
		}
	}
}
