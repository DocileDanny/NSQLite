# NSQLite [![Build Status](https://travis-ci.org/CsharpDatabase/CsharpSQLite.svg?branch=master)](https://travis-ci.org/CsharpDatabase/CsharpSQLite) [![Bitdeli Badge](https://d2weczhvl823v0.cloudfront.net/CsharpDatabase/csharpsqlite/trend.png)](https://bitdeli.com/free "Bitdeli Badge")

This is a pure C# SQLite implement. I forked it from [CsharpSQLite](https://github.com/CsharpDatabase/CsharpSQLite).
And its origin source is on [Google Code](http://code.google.com/p/csharp-sqlite/).
More infomation could find at [InfoQ](http://www.infoq.com/news/2009/08/SQLite-Has-Been-Ported-to-.NET).

I added a new feature of support REAL memory database. For the word REAL I mean it behaves just like file based SQLite. 

SQLite supports memory database as well. But it only created by new connection created and will dispose while the connection closed. And all your operation should use this connection. And if other thread want to use this connection will cause an error.

The new feature I added is to handles file access functions and port them to memory acess functions. The result will store in a Dictionary\<string, byte[]\>, it use the file name as key and file content as value. So we can visit the memory file multi times. And we can initialize it with a real file content instead of run initialize SQL statements. It just like a file but the speed will be a lot of fast than a file.

I tested it in my project [DbEntry.Net](https://github.com/Lifeng-Liang/DbEntry) for unit test. The file based SQLite costs about 50s and the memory version only costs about 10s. It also a little faster than file based SQLite on RamDisk.

The following code is the base class of my unit tests. The commented code is use file based SQLite.

````c#
public class DataTestBase : SqlTestBase
{
    private const string FileName = "UnitTest.db";
	private static readonly string TestFilePath = "/Volumes/RamDisk/" + FileName;
    private static readonly byte[] TestFileBuffer = ResourceHelper.ReadAll(typeof(DataTestBase), FileName);

    protected override void OnSetUp()
    {
		/*
        File.Delete(TestFilePath);
        using (Stream s = new FileStream(TestFilePath, FileMode.Create))
        {
            s.Write(TestFileBuffer, 0, TestFileBuffer.Length);
        }
		*/
		System.Data.SQLite.TypeHolder.Type = StreamType.Memory;
		var bs = new byte[TestFileBuffer.Length];
		TestFileBuffer.CopyTo(bs, 0);
		System.Data.SQLite.MemFileStream.Files [TestFilePath] = bs;
    }

    protected override void OnTearDown()
    {
		System.Data.SQLite.MemStreamHandler.Instance.Delete (TestFilePath);
		//File.Delete(TestFilePath);
    }
}
````

