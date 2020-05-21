

	public delegate void Callback();

	public delegate void Callback<T>(T arg1);

	public delegate void Callback<T, U>(T arg1, U arg2);

	public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

	public delegate void Callback<T, U, V, W>(T arg1, U arg2, V arg3, W arg4);

	//Add return Type
	public delegate T RCallback<T>();
	public delegate T RCallback<T, U>(U arg1);
	public delegate T RCallback<T, U, V>(U arg1, V arg2);
