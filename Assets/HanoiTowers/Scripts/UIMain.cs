using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HanoiTowers
{

	public class UIMain : MonoBehaviour
	{
		enum State
		{
			None, Menu, Game, Result
		}

		[SerializeField]
		private InputField _inputNum;		
		[SerializeField]
		private Field _field;

		private int _numRings = 0;

		[Header("Stats")]
		[SerializeField]
		private GameObject _menuState;
		[SerializeField]
		private GameObject _gameState;
		[SerializeField]
		private GameObject _resultState;

		private Dictionary<State, GameObject> _states = new Dictionary<State, GameObject>();

		private void Awake()
		{
			if( _menuState )
				_states.Add( State.Menu, _menuState );

			if( _gameState )
				_states.Add( State.Game, _gameState );

			if( _resultState )
				_states.Add( State.Result, _resultState );

			if( _field )
				_field.onComplete += () => GotoState( State.Result );
		}

		private void Start()
		{
			GotoState( State.Menu );
		}

		private void GotoState( State state )
		{
			foreach( var pair in _states )
			{
				pair.Value.SetActive( false );
			}

			GameObject stateObj;
			if( _states.TryGetValue( state, out stateObj ) )
				stateObj.SetActive( true );
		}

		#region Input Event

		public void OnEndEdit_InputCount( string text )
		{
			int num = 0;
			if( int.TryParse( text, out num ) )
			{
				_numRings = Mathf.Clamp( num, 1, Field.MAX_RINGS );
			}

			if( _inputNum )
				_inputNum.text = _numRings.ToString();
		}

		public void OnClick_DecideBtn()
		{
			if( _field )
			{
				if( _numRings > 0 )
				{
					_field.Decide( _numRings );

					GotoState( State.Game );
				}
				else
					Debug.LogError( "not valid count!" );
			}
		}

		public void OnClick_RestartBtn()
		{
			if( _field )
				_field.Stop();

			GotoState( State.Menu );
		}

		#endregion Input Event
	}
}