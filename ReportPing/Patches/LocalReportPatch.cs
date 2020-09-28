using System;
using HarmonyLib;
using UnityEngine;
using Utf8Json;

namespace ReportPing.Patches
{
	[HarmonyPatch(typeof(CheaterReport))]
	[HarmonyPatch(nameof(CheaterReport.SubmitReport))]
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
				string pings = "";

				foreach (string s in ReportPing.Instance.Config.RoleId)
				{
					pings += $"<@&{s}> ";
				}

				pings = Uri.EscapeDataString(pings);

				string payload = JsonSerializer.ToJsonString<DiscordWebhook>(new DiscordWebhook(
					pings, CheaterReport.WebhookUsername,
					CheaterReport.WebhookAvatar, false, new DiscordEmbed[1]
					{
						new DiscordEmbed(CheaterReport.ReportHeader, "rich", CheaterReport.ReportContent,
							CheaterReport.WebhookColor, new DiscordEmbedField[10]
							{
								new DiscordEmbedField("Server Name", CheaterReport.ServerName, false),
								new DiscordEmbedField("Server Endpoint", string.Format("{0}:{1}", (object) ServerConsole.Ip, (object) ServerConsole.Port), false),
								new DiscordEmbedField("Reporter UserID", CheaterReport.AsDiscordCode(reporterUserId), false),
								new DiscordEmbedField("Reporter Nickname", CheaterReport.DiscordSanitize(reporterNickname), false),
								new DiscordEmbedField("Reported UserID", CheaterReport.AsDiscordCode(reportedUserId), false),
								new DiscordEmbedField("Reported Nickname", CheaterReport.DiscordSanitize(reportedNickname), false),
								new DiscordEmbedField("Reported ID", reportedId.ToString(), false),
								new DiscordEmbedField("Reason", CheaterReport.DiscordSanitize(reason), false),
								new DiscordEmbedField("Timestamp", TimeBehaviour.Rfc3339Time(), false),
								new DiscordEmbedField("UTC Timestamp", TimeBehaviour.Rfc3339Time(DateTimeOffset.UtcNow), false)
							})
					}));
				HttpQuery.Post(friendlyFire ? FriendlyFireConfig.WebhookUrl : CheaterReport.WebhookUrl, "payload_json=" + payload);
				__state = true;
			}
			catch (Exception ex)
			{
				ServerConsole.AddLog("Failed to send report by webhook: " + ex.Message, ConsoleColor.Gray);
				Debug.LogException(ex);
				__state = false;
			}

			return false;
		}

		static void Postfix(ref bool __result, bool __state)
		{
			__result = __state;
		}
	}
}
