namespace Realm.Maps;

using System;
using Realm.MapAPI;

/// <summary>
///     The entry script of this board. The rules the maps it references state are compiled into this same assembly,
///     each under its own namespace, so this class starts those scripts and forwards the match's lifecycle to them.
///     This board owns the rules it states itself; the composed scripts own theirs.
/// </summary>
public class MapScript : IWasmModule, ILobbyScript
{
	private IMapScript[] _composedScripts = Array.Empty<IMapScript>();

	/// <summary>
	///     Starts the board and every ruleset composed into it, in the order the composition registry names them.
	/// </summary>
	/// <param name="api">The game API reference to manipulate the game world.</param>
	public void Initialize(IGameAPI api)
	{
		IMapScript[] scripts = ComposedScripts();
		for (int index = 0; index < scripts.Length; index++)
		{
			scripts[index].Initialize(api);
		}
	}

	/// <summary>
	///     Passes the tick to every composed ruleset, after this board has had its own say.
	/// </summary>
	/// <param name="api">The game API reference to manipulate the game world.</param>
	/// <param name="delta">The time elapsed since the last physics tick, in seconds.</param>
	public void Update(IGameAPI api, float delta)
	{
		IMapScript[] scripts = ComposedScripts();
		for (int index = 0; index < scripts.Length; index++)
		{
			scripts[index].Update(api, delta);
		}
	}

	/// <summary>
	///     Offers the roster this board seats, which is the roster its composed rulesets state.
	/// </summary>
	/// <param name="api">The roster this lobby currently holds.</param>
	public void LobbyInitialize(ILobbyAPI api)
	{
		IMapScript[] scripts = ComposedScripts();
		for (int index = 0; index < scripts.Length; index++)
		{
			if (scripts[index] is ILobbyScript lobbyScript) lobbyScript.LobbyInitialize(api);
		}
	}

	/// <summary>
	///     Keeps the roster current while players join, leave and change their seats.
	/// </summary>
	/// <param name="api">The roster this lobby currently holds.</param>
	/// <param name="deltaSeconds">The time since the previous lobby tick.</param>
	public void LobbyUpdate(ILobbyAPI api, float deltaSeconds)
	{
		IMapScript[] scripts = ComposedScripts();
		for (int index = 0; index < scripts.Length; index++)
		{
			if (scripts[index] is ILobbyScript lobbyScript) lobbyScript.LobbyUpdate(api, deltaSeconds);
		}
	}

	/// <summary>
	///     Collects the warning the host must read before the match starts. Every composed ruleset that warns has its
	///     say, and the host reads them joined, so a board cannot silence a ruleset that states the seats are unfair.
	/// </summary>
	/// <param name="api">The roster this lobby currently holds.</param>
	/// <param name="message">The warning the host should read, or empty when there is nothing to warn about.</param>
	/// <returns>Whether the host must read a warning before the match starts.</returns>
	public bool TryGetStartWarning(ILobbyAPI api, out string message)
	{
		message = string.Empty;
		bool warned = false;

		IMapScript[] scripts = ComposedScripts();
		for (int index = 0; index < scripts.Length; index++)
		{
			if (scripts[index] is not ILobbyScript lobbyScript) continue;
			if (!lobbyScript.TryGetStartWarning(api, out string warning)) continue;
			if (string.IsNullOrEmpty(warning)) continue;

			message = warned ? message + "\n" + warning : warning;
			warned = true;
		}

		return warned;
	}

	private IMapScript[] ComposedScripts()
	{
		if (_composedScripts.Length == 0)
		{
			_composedScripts = MapComposition.CreateScripts();
		}

		return _composedScripts;
	}
}
