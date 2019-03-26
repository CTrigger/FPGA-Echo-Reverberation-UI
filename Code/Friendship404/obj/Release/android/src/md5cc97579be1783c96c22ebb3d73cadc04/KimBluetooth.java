package md5cc97579be1783c96c22ebb3d73cadc04;


public class KimBluetooth
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Friendship404.Framework.KimBluetooth, Friendship404, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", KimBluetooth.class, __md_methods);
	}


	public KimBluetooth () throws java.lang.Throwable
	{
		super ();
		if (getClass () == KimBluetooth.class)
			mono.android.TypeManager.Activate ("Friendship404.Framework.KimBluetooth, Friendship404, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
