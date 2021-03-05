csc.exe /t:module code2.cs
csc.exe /t:module /addmodule:code2.netmodule code1.cs
al /out:result.exe /t:exe /main:Program.Main .\code2.netmodule .\code1.netmodule
