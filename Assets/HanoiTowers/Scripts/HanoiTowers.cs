using System.Collections.Generic;
using UnityEngine;

namespace HanoiTowers
{

	public class Step
	{
		public readonly int ring;
		public readonly int from;
		public readonly int to;

		public Step( int ring, int from, int to )
		{
			this.ring = ring;
			this.from = from;
			this.to = to;
		}
	}

	public class HanoiTowersAlgorithm
	{
		class Tower : Stack<int>
		{
			public readonly int id;

			public Tower( int id )
			{
				this.id = id;
			}
		}

		private List<Step> _steps = new List<Step>();

		public List<Step> Decide( int count )
		{
			_steps.Clear();

			Tower from = new Tower( 0 );
			Tower to = new Tower( 1 );
			Tower buf = new Tower( 2 );

			for( int i = 0; i < count; ++i )
				from.Push( i );

			Hanoi( from, buf, to, count );			

			Debug.LogFormat( "Decided! - steps: {0}", _steps.Count );

			return _steps;
		}

		private void Hanoi( Tower from, Tower buf, Tower to, int count )
		{
			if( count == 1 )
			{
				Swap( from, to );

				return;
			}

			Hanoi( from, to, buf, count - 1 );

			Swap( from, to );

			Hanoi( buf, from, to, count - 1 );
		}
				
		private void Swap( Tower from, Tower to )
		{
			int ring = from.Pop();

			to.Push( ring );

			_steps.Add( new Step( ring, from.id, to.id ) );
		}
	}
}