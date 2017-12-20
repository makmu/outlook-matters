﻿using System.Collections.Generic;
using System.Linq;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v4.Interface
{
    public static class JsonPayloadExtensions
    {
        public static string SerializeToPayload(this Login login)
        {
            return "{\"login_id\":\"" + login.LoginId + "\",\"password\":\"" + login.Password + "\"}";
        }

        public static string SerializeToPayload(this IEnumerable<Team> teamList)
        {
            var payload = "";
            var jsonListBegin = "[";
            var jsonListEnd = "]";

            var teams = teamList as Team[] ?? teamList.ToArray();

            payload += jsonListBegin;
            var firstItem = teams[0].SerializeToPayload();
            payload += firstItem;

            for (var i = 1; i < teams.Length; i++)
            {
                payload += ", ";
                payload += teams[i].SerializeToPayload();
            }

            payload += jsonListEnd;
            return payload;
        }

        public static string SerializeToPayload(this Team team)
        {
            return "{\"id\":\"" + team.Id + "\",\"name\":\"" + team.Name + "\"}";
        }
    }
}