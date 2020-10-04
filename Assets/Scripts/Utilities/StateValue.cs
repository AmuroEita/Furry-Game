using System;
using UnityEngine;

namespace JTUtility
{
	////For buff system, paused for now
	//public class BaseStateValie<T> where T : IComparable<T>
	//{
	//	[SerializeField]
	//	private T @base;
	//	public T Base
	//	{
	//		get { return @base; }
	//		set { @base = value; }
	//	}

	//	[SerializeField]
	//	private T maximum;
	//	public T Maximum
	//	{
	//		get { return maximum; }
	//		set { maximum = value; }
	//	}

	//	[SerializeField]
	//	private T current;
	//	public T Current
	//	{
	//		get { return current; }
	//		set
	//		{
	//			current = value;
	//		}
	//	}

	//	public BaseStateValie(T @base)
	//	{
	//		this.@base = @base;
	//		this.current = @base;
	//		this.maximum = @base;
	//	}

	//	public BaseStateValie(T @base, T current)
	//	{
	//		this.@base = @base;
	//		this.current = current;
	//		this.maximum = @base;
	//	}

	//	public void ResetCurrent()
	//	{
	//		current = maximum;
	//	}

	//	public void ResetMaximum()
	//	{
	//		maximum = @base;
	//	}

	//	public bool IsFull()
	//	{
	//		return maximum.Equals(current);
	//	}

	//	public static implicit operator T(BaseStateValie<T> value)
	//	{
	//		return value.current;
	//	}
	//}

    [Serializable]
    public class StateValue
    {
        [SerializeField]
        private float @base;
        public float Base
        {
            get { return @base; }
            set { @base = value; }
        }

		[SerializeField]
		private float maximum;
		public float Maximum
		{
			get { return maximum; }
			set { maximum = value; }
		}

		[SerializeField]
        private float current;
        public float Current
        {
            get { return current; }
            set
			{
				current = value;
				if (AutoClamp) Clamp();
			}
        }

		public float Percent
		{
			get => current / maximum;
			set => Current = maximum * value;
		}

		/// <summary>
		/// Limit <see cref="Current"/> in range of (0, <see cref="Base"/>) whenever it got modified.
		/// Default is true
		/// </summary>
		public bool AutoClamp { get; set; }

        public StateValue(float @base)
        {
            this.@base = @base;
            this.current = @base;
			this.maximum = @base;
			AutoClamp = true;
        }

        public StateValue(float @base, float current)
        {
            this.@base = @base;
            this.current = current;
			this.maximum = @base;
			AutoClamp = true;
		}

        public void ResetCurrent()
        {
            current = maximum;
        }

		public void ResetMaximum()
		{
			maximum = @base;
		}

        public bool IsFull()
        {
            return maximum - current <= float.Epsilon;
        }

        public void Clamp()
        {
            current = Mathf.Clamp(current, 0, maximum);
        }

        public static implicit operator float(StateValue value)
        {
            return value.current;
        }

		public static StateValue operator +(StateValue v1, float v2)
        {
            v1.current += v2;
			if (v1.AutoClamp) v1.Clamp();
			return v1;
        }

        public static StateValue operator -(StateValue v1, float v2)
        {
            v1.current -= v2;
			if (v1.AutoClamp) v1.Clamp();
			return v1;
        }

        public static StateValue operator *(StateValue v1, float v2)
        {
            v1.current *= v2;
			if (v1.AutoClamp) v1.Clamp();
			return v1;
        }

        public static StateValue operator /(StateValue v1, float v2)
        {
            v1.current /= v2;
			if (v1.AutoClamp) v1.Clamp();
			return v1;
        }
    }

	[Serializable]
	public class StateValueInt
	{
		[SerializeField]
		private int @base;
		public int Base
		{
			get { return @base; }
			set { @base = value; }
		}

		[SerializeField]
		private int maximum;
		public int Maximum
		{
			get { return maximum; }
			set { maximum = value; }
		}

		[SerializeField]
		private int current;
		public int Current
		{
			get { return current; }
			set
			{
				current = value;
				if (AutoClamp) Clamp();
			}
		}

		public float Percent
		{
			get => (float)current / maximum;
			set => current = (int)(maximum * value);
		}

		/// <summary>
		/// Limit <see cref="Current"/> in range of (0, <see cref="Base"/>) whenever it got modified.
		/// Default is true
		/// </summary>
		public bool AutoClamp { get; set; }

		public StateValueInt(int @base)
		{
			this.@base = @base;
			this.current = @base;
			this.maximum = @base;
			AutoClamp = true;
		}

		public StateValueInt(int @base, int current)
		{
			this.@base = @base;
			this.current = current;
			this.maximum = @base;
			AutoClamp = true;
		}

		public void ResetCurrentValue()
		{
			current = maximum;
		}

		public void ResetMaximum()
		{
			maximum = @base;
		}

		public bool IsFull()
		{
			return maximum == current;
		}

		public void Clamp()
		{
			current = Mathf.Clamp(current, 0, maximum);
		}

		public static implicit operator int(StateValueInt value)
		{
			return value.current;
		}

		public static StateValueInt operator +(StateValueInt v1, int v2)
		{
			v1.current += v2;
			if (v1.AutoClamp) v1.Clamp();
			return v1;
		}

		public static StateValueInt operator -(StateValueInt v1, int v2)
		{
			v1.current -= v2;
			if (v1.AutoClamp) v1.Clamp();
			return v1;
		}

		public static StateValueInt operator *(StateValueInt v1, int v2)
		{
			v1.current *= v2;
			if (v1.AutoClamp) v1.Clamp();
			return v1;
		}

		public static StateValueInt operator /(StateValueInt v1, int v2)
		{
			v1.current /= v2;
			if (v1.AutoClamp) v1.Clamp();
			return v1;
		}
	}
}