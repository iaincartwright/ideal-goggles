using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace iUtils
{
	class iProcessHelper
	{
		/// <summary>
		/// Kills a process and all it's child processes
		/// </summary>
		/// <param name="a_pid"></param>
		public static void TerminateId(int a_pid)
		{
			StartVisit();

			TerminateId((uint)a_pid);

			EndVisit();
		}

		[DllImport("kernel32.dll")]
		static extern IntPtr OpenProcess(uint a_dwDesiredAccess, bool a_bInheritHandle, uint a_dwProcessId);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(IntPtr a_hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool TerminateProcess(IntPtr a_hProcess, uint a_uExitCode);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr CreateToolhelp32Snapshot(uint a_dwFlags, uint a_th32ProcessId);

		[DllImport("kernel32.dll")]
		static extern bool Process32First(IntPtr a_hSnapshot, ref Processentry32 a_lppe);

		[DllImport("kernel32.dll")]
		static extern bool Process32Next(IntPtr a_hSnapshot, ref Processentry32 a_lppe);

		[StructLayout(LayoutKind.Sequential)]
		public struct Processentry32
		{
			public uint dwSize;
			public uint cntUsage;
			public uint th32ProcessID;
			public IntPtr th32DefaultHeapID;
			public uint th32ModuleID;
			public uint cntThreads;
			public uint th32ParentProcessID;
			public int pcPriClassBase;
			public uint dwFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szExeFile;
		};

		[Flags]
		enum Th32Cs
		{
			Snapheaplist = 1,
			Snapprocess = 2,
			Snapthread = 4,
			Snapmodule = 8,
			Snapmodule32 = 16,
			Inherit = unchecked((int)0x80000000),
		};

		[Flags]
		enum ProcessFlags
		{
			Terminate = 0x0001,
			CreateThread = 0x0002,
			SetSessionid = 0x0004,
			VmOperation = 0x0008,
			VmRead = 0x0010,
			VmWrite = 0x0020,
			DupHandle = 0x0040,
			CreateProcess = 0x0080,
			SetQuota = 0x0100,
			SetInformation = 0x0200,
			QueryInformation = 0x0400,
			SuspendResume = 0x0800,
		};

		const Th32Cs Snapall = (Th32Cs.Snapheaplist | Th32Cs.Snapprocess | Th32Cs.Snapthread | Th32Cs.Snapmodule);

		private static void TerminateId(uint a_pid)
		{
			var killStack = new Stack<uint>();

			s_findPid = a_pid;

			VisitEachProcess(AddChildrenToStack, killStack);

			while (killStack.Count > 0)
			{
				TerminateId(killStack.Pop());
			}

			var hProc = GetKillHandle(a_pid);

			TerminateWithExtremePrejudice(hProc);

			CloseHandle(hProc);
		}

		delegate bool ProcVisitor(Processentry32 a_info, object a_data);

		private static void StartVisit()
		{
			s_hSnapShot = CreateToolhelp32Snapshot((uint)Th32Cs.Snapprocess, 0);
		}

		private static void VisitEachProcess(ProcVisitor a_vistor, object a_data)
		{
			var info = new Processentry32 { dwSize = (uint)Marshal.SizeOf(typeof(Processentry32)) };

			if (!Process32First(s_hSnapShot, ref info))
			{
				throw new Exception("Cannot find first Process");
			}

			do
			{
				// callback if there is one
				if (a_vistor != null && a_vistor(info, a_data) == false)
					break;
			}
			while (Process32Next(s_hSnapShot, ref info));
		}

		private static void EndVisit()
		{
			CloseHandle(s_hSnapShot);
		}

		static IntPtr s_hSnapShot = IntPtr.Zero;
		static uint s_findPid = uint.MaxValue;

		private static bool AddChildrenToStack(Processentry32 a_info, object a_data)
		{
			var killStack = (Stack<uint>)a_data;

			if (a_info.th32ParentProcessID == s_findPid)
			{
				killStack.Push(a_info.th32ProcessID);
			}

			return true;
		}

		private static IntPtr GetKillHandle(uint a_pid)
		{
			var hProc = IntPtr.Zero;

			var accessFlags = (uint)(ProcessFlags.QueryInformation | ProcessFlags.Terminate);

			var info = new Processentry32();

			info.dwSize = (uint)Marshal.SizeOf(typeof(Processentry32));

			if (!Process32First(s_hSnapShot, ref info))
			{
				throw new Exception("Cannot find first Process");
			}

			do
			{
				if (info.th32ProcessID == a_pid)
				{
					hProc = OpenProcess(accessFlags, false, info.th32ProcessID);

					break;
				}
			}
			while (Process32Next(s_hSnapShot, ref info));

			return hProc;
		}

		private static bool TerminateWithExtremePrejudice(IntPtr a_handle)
		{
			return TerminateProcess(a_handle, 1);
		}
	}
}