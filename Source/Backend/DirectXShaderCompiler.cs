using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ShaderCompilerOnline.Source.Backend
{
	public class DirectXShaderCompiler
	{
		private ProcessStartInfo m_processStartInfo = null;

		public DirectXShaderCompiler(string path)
		{
			m_processStartInfo = new ProcessStartInfo(path);
			m_processStartInfo.UseShellExecute = false;
			m_processStartInfo.RedirectStandardOutput = true;
		}

		public string Start()
		{
			//			var process = Process.Start(m_processStartInfo);
			string output;
			using (var process = new Process())
			{
				process.StartInfo.FileName = @"D:\project\slowliver\ShaderCompilerOnline\External\DXC\v1.6.2112\dxc.exe";
				process.StartInfo.Arguments = "-help";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.Start();
				output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
			}
			return output;
		}
	}
}
