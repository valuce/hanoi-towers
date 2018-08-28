using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HanoiTowers
{

	public class Field : MonoBehaviour
	{
		public const int MAX_RINGS = 10;
		private readonly Vector3 RING_OFSSET = Vector3.up * 0.1f;
		private const float RING_SCALE_MIN = 1f;
		private const float RING_SCALE_MAX = 3f;
		private const float RING_SCALE_UP = 0.1f;
		private const float RING_HEIGHT = RING_SCALE_UP * 2f;

		public event System.Action onComplete;

		[SerializeField]
		private Transform RingPrefab;
		
		private Transform[] _towers;

		private readonly List<Transform> _rings = new List<Transform>();

		private readonly HanoiTowersAlgorithm _hanoiTowers = new HanoiTowersAlgorithm();

		private void Awake()
		{
			int towerCount = transform.childCount;

			_towers = new Transform[towerCount];

			for( int i = 0; i < towerCount; ++i )
			{
				_towers[i] = transform.GetChild( i );
			}
		}

		public void Stop()
		{
			StopAllCoroutines();

			foreach( Transform ring in _rings )
			{
				Destroy( ring.gameObject );
			}

			_rings.Clear();
		}

		public void Decide( int count )
		{
			Stop();

			if( _towers.Length < 3 || RingPrefab == null )
				return;

			Transform towerFrom = _towers[0];

			for( int i = 0; i < count; ++i )
			{
				var ring = Instantiate( RingPrefab, transform, false );
				ring.position = GetRingPosition( towerFrom );

				// Set ring scale
				float scale = Mathf.Lerp( RING_SCALE_MIN, RING_SCALE_MAX, (count - (i+1f)) / count );
				ring.localScale = new Vector3( scale, RING_SCALE_UP, scale );

				// Set ring color
				Renderer renderer = ring.GetComponent<Renderer>();
				if( renderer && count > 1 )
					renderer.material.color = Color.Lerp( Color.red, Color.yellow, i / (count - 1f) );

				ring.SetParent( towerFrom );
				_rings.Add( ring );
			}

			var steps = _hanoiTowers.Decide( count );
			
			StartCoroutine( ShowSteps( steps ) );
		}

		private Vector3 GetRingPosition( Transform tower )
		{
			return tower.position + (Vector3.up * tower.childCount * RING_HEIGHT) + RING_OFSSET;
		}

		private Vector3 GetUpPosition( Transform tower )
		{
			return tower.position + (Vector3.up * MAX_RINGS * RING_HEIGHT) + RING_OFSSET;
		}

		IEnumerator ShowSteps( List<Step> steps )
		{
			yield return new WaitForSeconds( 0.25f );

			foreach( Step step in steps )
			{
				var ring = _rings[step.ring];
				var towerTo = _towers[step.to];
				var towerFrom = _towers[step.from];

				if( !ring || !towerFrom || !towerTo )
					continue;

				// up
				yield return RingMoveTo( ring, GetUpPosition( towerFrom ), 0.2f );

				// to
				yield return RingMoveTo( ring, GetUpPosition( towerTo ), 0.5f );

				// down
				yield return RingMoveTo( ring, GetRingPosition( towerTo ), 0.2f );

				ring.SetParent( towerTo );
			}

			if( onComplete != null )
				onComplete.Invoke();
		}

		IEnumerator RingMoveTo( Transform ring, Vector3 positionTo, float timeFx )
		{
			Vector3 positionFrom = ring.position;
			float timer = 0f;
			while( true )
			{
				timer += Time.deltaTime;

				ring.position = Vector3.Lerp( positionFrom, positionTo, timer / timeFx );

				if( timer >= timeFx )
					break;

				yield return 0;
			}
		}

		public void SetTimeScale( float timeScale )
		{
			Time.timeScale = timeScale;
		}
	}
}