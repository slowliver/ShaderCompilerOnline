using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ShaderCompilerOnline.Page
{
	public class InputData
	{
		public string text{ set; get; }
	}

	public class OutputData
	{
		public string text { set; get; }
	}

	public class IndexModel : PageModel
	{
		[BindProperty]
		public InputData inputData { get; set; }

		[BindProperty]
		public InputData outputData { get; set; }

		private readonly ILogger<IndexModel> _logger;

		public IndexModel(ILogger<IndexModel> logger)
		{
			_logger = logger;
			inputData = new InputData();
			inputData.text = @"struct PSInput
{
	float2 uv : TEXCOORD0;
};

float4 PSMain(PSInput input) : SV_TARGET
{
	return float4(input.uv, 0.0f, 1.0f);
}";
		}

		public void OnGet()
		{
		}

		public void OnPostContext()
		{
			// here I can put something to execute 
			// when the submit button is clicked
			var dxc = new ShaderCompilerOnline.Source.Backend.DirectXShaderCompiler(Path.Combine(Directory.GetCurrentDirectory(), "External/DXC/v1.6.2112/dxc.exe"));
			var result = dxc.Start();
			outputData.text = result;
		//	return new ViewResult();// (inputData.text);
		}

	}
}
