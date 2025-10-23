using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLevel;

public readonly struct FileAccessResult(bool success,string message) {
	public readonly string Message = message;
	public readonly bool IsSuccess = success;
	public FileAccessResult() : this(true,"") { }
}