using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RemoteObject : MarshalByRefObject
{
	public RemoteObject()
	{
	}

	public virtual string Execute(string name)
	{

		Console.WriteLine(name);

		return "Hello," + name;
	}
}
