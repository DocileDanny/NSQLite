using System;
using System.IO;
using System.Collections.Generic;

namespace System.Data.SQLite
{
	public enum StreamType
	{
		File,
		Memory,
	}

	public static class TypeHolder
	{
		public static StreamType Type = StreamType.File;
	}

	public class StreamHandler
	{
		public static readonly StreamHandler Instance;

		static StreamHandler ()
		{
			Instance = (TypeHolder.Type == StreamType.File) ?
				new StreamHandler() : new MemStreamHandler();
		}
		
		public virtual Stream Create(string fileName)
		{
			return File.Create(fileName);
		}

		public virtual Stream New(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
		{
			return new FileStream(path, mode, access, share, bufferSize, options);
		}

		public virtual bool Exists(string path)
		{
			return File.Exists(path);
		}

		public virtual void Delete(string path)
		{
			File.Delete(path);
		}

		public virtual FileAttributes GetAttributes(string path)
		{
			return File.GetAttributes(path);
		}

		public virtual string GetFullPath(string path)
		{
			return Path.GetFullPath(path);
		}

		public virtual string GetTempPath()
		{
			return Path.GetTempPath();
		}

		public virtual string GetTempFileName()
		{
			return Path.GetTempFileName();
		}

		public virtual string Combine(string path1, string path2)
		{
			return Path.Combine(path1, path2);
		}

		public virtual IntPtr GetHandle(Stream s)
		{
			return ((FileStream)s).Handle;
		}

		public virtual void Lock(Stream s, long offset, long length)
		{
			((FileStream)s).Lock(offset, length);
		}

		public virtual void Unlock(Stream s, long offset, long length)
		{
			((FileStream)s).Unlock(offset, length);
		}
	}

	public class MemStreamHandler : StreamHandler
	{
		private static int _count = 0;
		
		public override Stream Create(string fileName)
		{
			return new MemFileStream(fileName);
		}

		public override Stream New(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
		{
			return new MemFileStream(path, mode, access, share, bufferSize, options);
		}

		public override bool Exists(string path)
		{
			return MemFileStream.Files.ContainsKey(path);
		}

		public override void Delete(string path)
		{
			if(MemFileStream.Files.ContainsKey(path))
			{
				MemFileStream.Files.Remove(path);
			}
		}

		public override FileAttributes GetAttributes(string path)
		{
			return FileAttributes.Normal;
		}

		public override string GetFullPath(string path)
		{
			return path;
		}

		public override string GetTempPath()
		{
			return "";
		}

		public override string GetTempFileName()
		{
			_count++;
			return "$$$" + _count;
		}

		public override string Combine(string path1, string path2)
		{
			return path1 + path2;
		}

		public override IntPtr GetHandle(Stream s)
		{
			return (IntPtr)((MemFileStream)s)._path.GetHashCode();
		}

		public override void Lock(Stream s, long offset, long length)
		{
		}

		public override void Unlock(Stream s, long offset, long length)
		{
		}
	}

	public class MemFileStream : MemoryStream
	{
		public static readonly Dictionary<string, byte[]> Files = new Dictionary<string, byte[]>();

		internal string _path;

		public MemFileStream(string path)
		{
			_path = path;
			byte[] bs;
			if(Files.TryGetValue(_path, out bs))
			{
				this.Write(bs, 0, bs.Length);
			}
		}
		
		public MemFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
			:this(path)
		{
		}

		public override void Close()
		{
			Files[_path] = this.GetBuffer();
			base.Close();
		}
	}
}
