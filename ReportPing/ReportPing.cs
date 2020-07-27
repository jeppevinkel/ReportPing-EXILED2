﻿using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events;
using Exiled.Loader;
using HarmonyLib;

namespace ReportPing
{
	public class ReportPing : Plugin<Config>
	{
		private static readonly Lazy<ReportPing> LazyInstance = new Lazy<ReportPing>(() => new ReportPing());

		private ReportPing()
		{

		}

		/// <summary>
		/// Gets the lazy instance.
		/// </summary>
		public static ReportPing Instance => LazyInstance.Value;

		public override PluginPriority Priority { get; } = PluginPriority.Low;

		/// <summary>
		/// The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
		/// </summary>
		private int _patchesCounter;

		/// <summary>
		/// Gets the <see cref="HarmonyLib.Harmony"/> instance.
		/// </summary>
		public Harmony Harmony { get; private set; }

		public override void OnEnabled()
		{
			base.OnEnabled();

			Patch();

			Log.Debug($"I will ping <@&{Config.RoleID}> whenever a report is posted over discord webhook.");
			Log.Debug($"I will ping <@&{ReportPing.Instance.Config.RoleID}> whenever a report is posted over discord webhook.");
		}

		public override void OnDisabled()
		{
			base.OnDisabled();

			Unpatch();
		}

		/// <summary>
		/// Registers the plugin events.
		/// </summary>
		private void RegisterEvents()
		{
			
		}

		/// <summary>
		/// Unregisters the plugin events.
		/// </summary>
		private void UnregisterEvents()
		{
			
		}

		/// <summary>
		/// Patches all events.
		/// </summary>
		public void Patch()
		{
			try
			{
				Harmony = new Harmony($"jopo.reportping.{++_patchesCounter}");
#if DEBUG
				var lastDebugStatus = Harmony.DEBUG;
				Harmony.DEBUG = true;
#endif
				Harmony.PatchAll();
#if DEBUG
				Harmony.DEBUG = lastDebugStatus;
#endif
				Log.Debug("Events patched successfully!", Loader.ShouldDebugBeShown);
			}
			catch (Exception exception)
			{
				Log.Error($"Patching failed! {exception}");
			}
		}

		/// <summary>
		/// Unpatches all events.
		/// </summary>
		public void Unpatch()
		{
			Log.Debug("Unpatching events...", Loader.ShouldDebugBeShown);

			Harmony.UnpatchAll();

			Log.Debug("All events have been unpatched complete. Goodbye!", Loader.ShouldDebugBeShown);
		}
	}
}