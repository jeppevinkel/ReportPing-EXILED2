using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Exiled.API.Features;
using Exiled.Loader;
using HarmonyLib;
using UnityEngine;
using Utf8Json;

namespace ReportPing.Patches
{
	[HarmonyPatch(typeof(CheaterReport))]
	[HarmonyPatch(nameof(CheaterReport.SubmitReport))]
	//[HarmonyPatch(new Type[] { typeof(string), typeof(string),
	//	typeof(string), typeof(int*), typeof(string), typeof(string), typeof(bool) })]
	internal static class LocalReportPatch
	{
		static bool Prefix(
			ref bool __state,
			string reporterUserId,
			string reportedUserId,
			string reason,
			ref int reportedId,
			string reporterNickname,
			string reportedNickname,
			bool friendlyFire)
		{
			try
			{
				string payload = JsonSerializer.ToJsonString<DiscordWebhook>(new DiscordWebhook(
					$"<@&{ReportPing.Instance.Config.RoleID}>", CheaterReport.WebhookUsername,
					CheaterReport.WebhookAvatar, false, new DiscordEmbed[1]
					{
						new DiscordEmbed(CheaterReport.ReportHeader, "rich", CheaterReport.ReportContent,
							CheaterReport.WebhookColor, new DiscordEmbedField[10]
							{
								new DiscordEmbedField("Server Name", CheaterReport.ServerName, false),
								new DiscordEmbedField("Server Endpoint",
									string.Format("{0}:{1}", (object) ServerConsole.Ip, (object) ServerConsole.Port),
									false),
								new DiscordEmbedField("Reporter UserID", CheaterReport.AsDiscordCode(reporterUserId),
									false),
								new DiscordEmbedField("Reporter Nickname",
									CheaterReport.DiscordSanitize(reporterNickname), false),
								new DiscordEmbedField("Reported UserID", CheaterReport.AsDiscordCode(reportedUserId),
									false),
								new DiscordEmbedField("Reported Nickname",
									CheaterReport.DiscordSanitize(reportedNickname), false),
								new DiscordEmbedField("Reported ID", reportedId.ToString(), false),
								new DiscordEmbedField("Reason", CheaterReport.DiscordSanitize(reason), false),
								new DiscordEmbedField("Timestamp",
									TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss.fff zzz"), false),
								new DiscordEmbedField("UTC Timestamp",
									TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss.fffZ", DateTimeOffset.UtcNow), false)
							})
					}));
				HttpClient client = new HttpClient();
				var content = new StringContent(payload, Encoding.UTF8, "application/json");
				var result = client.PostAsync(friendlyFire ? FriendlyFireConfig.WebhookUrl : CheaterReport.WebhookUrl, content).Result;
				__state = true;
			}
			catch (Exception ex)
			{
				ServerConsole.AddLog("Failed to send report by webhook: " + ex.Message, ConsoleColor.Gray);
				Debug.LogException(ex);
				__state = false;
			}

			Log.Debug($"LocalReport prefix with the state {__state} sent to <@&{ReportPing.Instance.Config.RoleID}>.", Loader.ShouldDebugBeShown);
			return false;
		}

		static void Postfix(ref bool __result, bool __state)
		{
			__result = __state;
			Log.Debug($"LocalReport postfix with return value {__result} sent to <@&{ReportPing.Instance.Config.RoleID}>.", Loader.ShouldDebugBeShown);
		}
	}
}
