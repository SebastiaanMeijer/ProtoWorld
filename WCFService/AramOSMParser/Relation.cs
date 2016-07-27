/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Aram.OSMParser
{
    public class Relation
    {
        public String id { get; set; }
        public List<Member> Members;
        public Relation(String id)
        {
            this.id = id;
            Members = new List<Member>();
        }

        public List<Tag> GetTagsPostgreSQL(String connectionString)
        {
            return GetTagsPostgreSQL(id, connectionString);
        }
        public List<Tag> GetTagsPostgreSQL(NpgsqlConnection con)
        {
            return GetTagsPostgreSQL(id, con);
        }
        public List<Tag> GetTagsPostgreSQL(String relationId, String connectionString)
        {
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            var res=GetTagsPostgreSQL(relationId, con);
            con.Close();
            return res;
        }
        public List<Tag> GetTagsPostgreSQL(String relationId, NpgsqlConnection con)
        {
            List<Tag> temp = new List<Tag>();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "select RelationId,\"name\",Info from viewRelationTagInfoAram where RelationId='" + relationId + "'";
            NpgsqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                Tag tempTag = new Tag();
                tempTag.KeyValueSQL = new string[] { reader["name"].ToString(), reader["Info"].ToString() };
                temp.Add(tempTag);
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            
            return temp;
        }

        public List<Tag> GetTagsSQL(String connectionString)
        {
            return GetTagsSQL(id, connectionString);
        }
        public List<Tag> GetTagsSQL(String relationId, String connectionString)
        {
            List<Tag> temp = new List<Tag>();

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand com = new SqlCommand("", con);
            com.CommandText = "select RelationId,Name,Info from RelationTagInfoAram where RelationId='" + relationId + "'";
            SqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                Tag tempTag = new Tag();
                tempTag.KeyValueSQL = new string[] { reader["Name"].ToString(), reader["Info"].ToString() };
                temp.Add(tempTag);
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            con.Close();
            return temp;
        }
        public int GetMemberType(String type)
        {
            switch (type.ToLower())
            {
                case "node":
                case "0":
                    return 0;
                case "way":
                case "1":
                    return 1;
                case "relation":
                case "2":
                    return 2;
                default: return -1;
            }
        }

        public List<Member> GetMembersPostgreSQL(String connectionString)
        {
            return GetMembersPostgreSQL(id, connectionString);
        }
        public List<Member> GetMembersPostgreSQL(NpgsqlConnection con)
        {
            return GetMembersPostgreSQL(id, con);
        }
        public List<Member> GetMembersPostgreSQL(String relationId, String connectionString)
        {
            List<Member> memberList = new List<Member>();
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            var res = GetMembersPostgreSQL(relationId, con);
            con.Close();
            return res;
        }
        public List<Member> GetMembersPostgreSQL(String relationId, NpgsqlConnection con)
        {
            List<Member> memberList = new List<Member>();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "SELECT RelationId,\"ref\",\"type\",\"role\",\"sort\" FROM viewRelationInfoAram where RelationId='" + relationId + "' order by sort";
            NpgsqlDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Member tempMember = new Member();
                    tempMember.ReferenceId = reader["ref"].ToString();
                    tempMember.RelationId = relationId; //reader["relationId"].ToString();
                    tempMember.Type = GetMemberType(reader["type"].ToString());
                    tempMember.Role = reader["role"].ToString();
                    tempMember.order = int.Parse(reader["sort"].ToString(), CultureInfo.InvariantCulture);
                    memberList.Add(tempMember);
                }
                if (!reader.IsClosed)
                    reader.Close();

                return memberList;
            }
            else
            {
                if (!reader.IsClosed)
                    reader.Close();

                return null;
            }
        }
        public List<Member> GetMembersPostgreSQLByType(String relationId, int memberType, String connectionString)
        {
            List<Member> memberList = new List<Member>();
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            var res = GetMembersPostgreSQLByType(relationId, memberType, con);
            con.Close();
            return res;
        }
        public List<Member> GetMembersPostgreSQLByType(String relationId, int memberType, NpgsqlConnection con)
        {
            List<Member> memberList = new List<Member>();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText =
                "SELECT RelationId,\"ref\",\"type\",\"role\",\"sort\" FROM viewRelationInfoAram where " +
                "RelationId='" + relationId + "' " +
                "AND \"type\"=" + memberType + " order by sort";
            NpgsqlDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Member tempMember = new Member();
                    tempMember.ReferenceId = reader["ref"].ToString();
                    tempMember.RelationId = relationId; //reader["relationId"].ToString();
                    tempMember.Type = GetMemberType(reader["type"].ToString());
                    tempMember.Role = reader["role"].ToString();
                    tempMember.order = int.Parse(reader["sort"].ToString(), CultureInfo.InvariantCulture);
                    memberList.Add(tempMember);
                }
                if (!reader.IsClosed)
                    reader.Close();
                return memberList;
            }
            else
            {
                if (!reader.IsClosed)
                    reader.Close();
                return null;
            }
        }
        /// <summary>
        /// An enumeration that is used when retreving route paths from OSM
        /// </summary>
        public enum RouteCorrection
        {
            /// <summary>
            /// Removes all the duplicates from the path.
            /// </summary>
            RemoveDuplicates,
            /// <summary>
            /// Fixes the lines that were draw in reverse direction of the path.
            /// </summary>
            FixReversedLine,
            /// <summary>
            /// Removes dead-end lines in the incorrectly defined paths as much as possible.
            /// </summary>
            RemoveDeadends
        }
        /// <summary>
        /// Gets the route path of an OSM route.
        /// </summary>
        /// <param name="routeId">The osm id of the route(relation)</param>
        /// <param name="memberType">The type of members to retrieve (This parameter is currently not used.).</param>
        /// <param name="connectionString">The connection string of the database</param>
        /// <returns>Return a list of nodes</returns>
        /// <seealso cref="GetRoutePath"/>
        public static List<OsmNode> GetRoutePathExperimental(string routeId, int memberType, String connectionString)
        {
            Aram.OSMParser.Relation rel = new Aram.OSMParser.Relation(routeId);
            var membersTemp = rel.GetMembersPostgreSQL(connectionString);
            List<OsmNode> nodesTemp = new List<OsmNode>();
            // Getting ways
            var mt2 = membersTemp.Where(mt => mt.Type == 1).ToList();
            // Exclude areas
            List<Member> tempWayList = new List<Member>();
            Tag AreaTag = new Tag() { KeyValueSQL = new string[] { "area", "yes" } };


            foreach (var m in mt2)
            {
                Aram.OSMParser.Way w = new Aram.OSMParser.Way(m.ReferenceId);
                var tags = w.GetTagsStockholmPostgreSQL(connectionString);

                if (tags.Where(wt => wt.KeyValueSQL[0] == AreaTag.KeyValueSQL[0] && wt.KeyValueSQL[1] == AreaTag.KeyValueSQL[1]).Count() == 0)
                {
                    tempWayList.Add(m);
                }
            }
            mt2 = tempWayList;

            Aram.OSMParser.Way wtemp = new Aram.OSMParser.Way();

            var AllWaysNodes = mt2.Select(
                wt => wtemp.GetNodesPostgreSQL(wt.ReferenceId, connectionString).ToArray()).ToArray();


            for (int workit = 0; workit < mt2.Count; workit++)
            {
                //var currentLine = GlobalSimulationPlannerBaseClass.client
                //	.GetWayNodes(mt2[workit].ReferenceId, wcfCon).ToArray();
                var currentLine = AllWaysNodes[workit];

                if (workit > 1)
                {
                    if (nodesTemp[nodesTemp.Count - 1].PositionSQL.Lat == currentLine[0].PositionSQL.Lat
                     && nodesTemp[nodesTemp.Count - 1].PositionSQL.Lon == currentLine[0].PositionSQL.Lon)
                    {
                        nodesTemp.AddRange(currentLine);
                    }
                    else
                    {
                        var rev = new OsmNode[currentLine.Length];
                        for (int revidx = 0; revidx < rev.Length; revidx++)
                            rev[revidx] = currentLine[rev.Length - revidx - 1];
                        nodesTemp.AddRange(rev);
                        //Debug.Log(mt2[workit].ReferenceId + " Reverse!");
                    }
                }
                else if (workit == 1)
                {
                    if (nodesTemp[nodesTemp.Count - 1].PositionSQL.Lat == currentLine[0].PositionSQL.Lat
                     && nodesTemp[nodesTemp.Count - 1].PositionSQL.Lon == currentLine[0].PositionSQL.Lon)
                    {
                        nodesTemp.AddRange(currentLine);
                        // Add the line normally.
                    }
                    else if (nodesTemp[0].PositionSQL.Lat == currentLine[currentLine.Length - 1].PositionSQL.Lat
                     && nodesTemp[0].PositionSQL.Lon == currentLine[currentLine.Length - 1].PositionSQL.Lon)
                    {
                        // Reverse the first line
                        var rev = new OsmNode[nodesTemp.Count];
                        for (int revidx = 0; revidx < rev.Length; revidx++)
                            rev[revidx] = nodesTemp[rev.Length - revidx - 1];
                        nodesTemp.Clear();
                        nodesTemp.AddRange(rev);
                        nodesTemp.AddRange(currentLine);
                    }
                    else
                    {
                        // Reverse the current line
                        var rev = new OsmNode[currentLine.Length];
                        for (int revidx = 0; revidx < rev.Length; revidx++)
                            rev[revidx] = currentLine[rev.Length - revidx - 1];
                        nodesTemp.AddRange(rev);
                    }
                }
                else
                    nodesTemp.AddRange(currentLine);

            }

            return nodesTemp;
        }
        /// <summary>
        /// Gets the route path of an OSM route.
        /// </summary>
        /// <param name="routeId">The osm id of the route(relation)</param>
        /// <param name="memberType">The type of members to retrieve (This parameter is currently not used.).</param>
        /// <param name="connectionString">The connection string of the database</param>
        /// <param name="correction">The correction types to be applied.</param>
        /// <returns>Return a list of nodes</returns>
        /// <seealso cref="GetRoutePathExperimental"/>
        public static List<OsmNode> GetRoutePath(string routeId, int memberType, String connectionString, RouteCorrection correction = RouteCorrection.RemoveDuplicates| RouteCorrection.FixReversedLine|RouteCorrection.RemoveDeadends)
        {
            Aram.OSMParser.Relation rell = new Aram.OSMParser.Relation(routeId);
            var members = rell.GetMembersPostgreSQLByType(routeId, memberType, connectionString);
            var way = new Aram.OSMParser.Way();
            var withoutAreas = members.Where(
                m =>
                    way.GetTagsStockholmPostgreSQL(m.ReferenceId, connectionString)
                    .Where(w => w.KeyValueSQL[0] == "area" && w.KeyValueSQL[1] == "yes").Count() == 0);
            var sorted = withoutAreas.ToList();
            sorted.Sort((m1, m2) => m1.order.CompareTo(m2.order));

            var Nodes = sorted.Select(w => way.GetNodesPostgreSQL(w.ReferenceId, connectionString)).ToList();
            OsmNode start;
            OsmNode end;

            // Finding all the closed paths such as squares, roundabout or circles.
            var closedLines = new List<Member>();
            for (int i = 0; i < sorted.Count; i++)
            {
                start = Nodes[i][0];
                end = Nodes[i][Nodes[i].Count - 1];
                if (start.EqualsById(end))
                    closedLines.Add(sorted[i]);
            }


            var original = sorted;
            var tempList = new List<Member>();
            // Remove deadends.

            bool deadendFound = true;


            bool closed = false;
            bool prevclosed = false;
            bool nextclosed = false;
            // Recursive removal of deadends
            if (correction == RouteCorrection.RemoveDeadends)
            {
                // while (deadendFound)
                {
                    deadendFound = false;
                    if (tempList.Count != 0)
                    {
                        original = new List<Member>(tempList);
                        original.Sort((m1, m2) => m1.order.CompareTo(m2.order));
                    }
                    tempList.Clear();
                    tempList.Add(original[0]);
                    Nodes = original.Select(w => way.GetNodesPostgreSQL(w.ReferenceId, connectionString)).ToList();
                    for (int i = 1; i < original.Count - 1; i++)
                    {
                        // CHEK IF ANY OF THE INSTANCES IS A closed line
                        start = Nodes[i][0];
                        end = Nodes[i][Nodes[i].Count - 1];
                        closed = closedLines.Contains(original[i]);
                        prevclosed = closedLines.Contains(original[i - 1]);
                        nextclosed = closedLines.Contains(original[i + 1]);
                        if (!(closed || prevclosed || nextclosed))
                        {
                            var startFound =
                                Nodes.Where(n => (
                                    (n[0].EqualsById(start) || n[n.Count - 1].EqualsById(start))
                                && !(n[0].EqualsById(start) && n[n.Count - 1].EqualsById(end))
                                )).Count() != 0;
                            var endFound =
                                Nodes.Where(n => (
                                    (n[0].EqualsById(end) || n[n.Count - 1].EqualsById(end))
                                && !(n[0].EqualsById(start) && n[n.Count - 1].EqualsById(end))
                                )).Count() != 0;

                            if (startFound && endFound)
                                tempList.Add(original[i]);
                            else
                            {
                                deadendFound = true;
                            }
                        }
                        else
                        {
                            // Search all the nodes on the line to find connections.
                            var startFound =
                                Nodes[i].Any(n => Nodes[i - 1].ContainsOsmNodeById(n));
                            var endFound =
                                Nodes[i].Any(n => Nodes[i + 1].ContainsOsmNodeById(n));
                            if (startFound && endFound)
                                tempList.Add(original[i]);
                        }
                    }
                    tempList.Add(original[original.Count - 1]);
                }

                Nodes = tempList.Select(w => way.GetNodesPostgreSQL(w.ReferenceId, connectionString)).ToList();
            }
            //closedLines.Clear();
            //for (int i = 0; i < tempList.Count; i++)
            //{
            //    start = Nodes[i][0];
            //    end = Nodes[i][Nodes[i].Count - 1];
            //    if (start.EqualsById(end))
            //        closedLines.Add(sorted[i]);
            //}

            //string route = "";
            List<OsmNode[]> correctPath = new List<OsmNode[]>();
            //route = tempList[0].RelationId;
            correctPath.Add(Nodes[0].ToArray());
            for (int i = 1; i < Nodes.Count; i++)
            {
                // It could be a circle
                var isCircle = Nodes[i][0].EqualsById(Nodes[i][Nodes[i].Count - 1]);
                var isPrevCircle = Nodes[i - 1][0].EqualsById(Nodes[i - 1][Nodes[i - 1].Count - 1]);
                var isReverse = Nodes[i][Nodes[i].Count - 1].EqualsById(Nodes[i - 1][Nodes[i - 1].Count - 1]);
                if (!isCircle && !isReverse)
                {
                    // Draw normally

                    correctPath.Add(Nodes[i].ToArray());
                }
                else
                {
                    if (isCircle)
                    {
                        // Find the correct portion and draw it
                        // Intersection with the previous line
                        OsmNode intersectionWithPrevious =
                            Nodes[i].Intersect(Nodes[i - 1],
                            new LambdaComparer<OsmNode>((o1, o2) => o1.EqualsById(o2))
                            ).FirstOrDefault();
                        OsmNode intersectionWithNext = null;
                        if (i < Nodes.Count - 1)
                            intersectionWithNext = Nodes[i].Intersect(Nodes[i + 1],
                                new LambdaComparer<OsmNode>((o1, o2) => o1.EqualsById(o2))
                                ).FirstOrDefault();

                        int firstNodeIndex;
                        int LastNodeIndex;

                        if (i < Nodes.Count - 1)
                        { // If it is not the last line.
                            firstNodeIndex = intersectionWithPrevious.order < intersectionWithNext.order ? intersectionWithPrevious.order : intersectionWithNext.order;
                            LastNodeIndex = intersectionWithPrevious.order > intersectionWithNext.order ? intersectionWithPrevious.order : intersectionWithNext.order;
                        }
                        else
                        {
                            // If it is the last line
                            firstNodeIndex = intersectionWithPrevious.order < Nodes[i][Nodes[i].Count - 1].order ? intersectionWithPrevious.order : Nodes[i][Nodes[i].Count - 1].order;
                            LastNodeIndex = intersectionWithPrevious.order > Nodes[i][Nodes[i].Count - 1].order ? Nodes[i][Nodes[i].Count - 1].order : intersectionWithPrevious.order;
                        }
                        var tempNode = new List<OsmNode>();
                        // TODO
                        // If first and last are the same then the line is the last and is a circle, draw normally.
                        if (firstNodeIndex == LastNodeIndex)
                        {
                            // Draw normally

                            correctPath.Add(Nodes[i].ToArray());
                        }
                        else
                        {
                            var CirclePortion = new List<OsmNode>();
                            Nodes[i].Sort((m1, m2) => m1.order.CompareTo(m2.order));
                            int idx1 = Nodes[i].IndexOf(Nodes[i].Where(n => n.order == firstNodeIndex).FirstOrDefault());
                            int idx2 = Nodes[i].IndexOf(Nodes[i].Where(n => n.order == LastNodeIndex).FirstOrDefault());

                            for (int idx = idx1; i <= idx2; idx++)
                                CirclePortion.Add(Nodes[i][idx]);

                            correctPath.Add(CirclePortion.ToArray());
                        }
                    }
                    else if (isPrevCircle && !isReverse)
                    {
                        // Draw normally

                        correctPath.Add(Nodes[i].ToArray());
                    }
                    else if (!isPrevCircle && isReverse)
                    {
                        var rev = new OsmNode[Nodes[i].Count];
                        for (int revidx = 0; revidx < rev.Length; revidx++)
                            rev[revidx] = Nodes[i][rev.Length - revidx - 1];

                        correctPath.Add(rev);
                    }
                    else
                    {
                        // It is not a reversed line, nor it is a circle or a previous is a circle
                    }
                }
            }
            List<OsmNode> result = new List<OsmNode>();
            foreach (var w in correctPath)
                result.AddRange(w);
            if (correction == RouteCorrection.RemoveDuplicates)
            {
                // Remove immediate duplicates
                List<OsmNode> duplicateFree = new List<OsmNode>();
                duplicateFree.Add(result[0]);
                foreach (var w in result)
                    if (!duplicateFree[duplicateFree.Count - 1].EqualsByGeoPosition(w))
                        duplicateFree.Add(w);
                result = duplicateFree;
            }
            return result;
        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="memberIds"></param>
        /// <param name="memberType"></param>
        /// <param name="memberRole"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<string> GetRelationsPostgreSQL(string[] memberIds, int memberType, string memberRole, String connectionString)
        {
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            con.Open();
            NpgsqlCommand com = new NpgsqlCommand("", con);
            com.CommandText = "SELECT DISTINCT relation_id FROM relation_members WHERE member_id IN ({MEMBERIDS}) AND member_type={MEMBERTYPE} ";
            string memberids = "";
            foreach (var memid in memberIds)
                memberids += memid + ", ";
            memberids = memberids.Substring(0, memberids.Length - 2);
            com.CommandText = com.CommandText.Replace("{MEMBERIDS}", memberids);
            com.CommandText = com.CommandText.Replace("{MEMBERTYPE}", memberType.ToString());
            if (!string.IsNullOrEmpty(memberRole))
                com.CommandText += "AND member_role='" + memberRole + "'";

            List<string> result = new List<string>();
            NpgsqlDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    result.Add(reader["relation_id"].ToString());
                }
                if (!reader.IsClosed)
                    reader.Close();
                con.Close();
                return result;
            }
            else
            {
                if (!reader.IsClosed)
                    reader.Close();
                con.Close();
                return null;
            }
        }

        public List<Member> GetMembersSQL(String connectionString)
        {
            return GetMembersSQL(id, connectionString);
        }

        public List<Member> GetMembersSQL(String relationId, String connectionString)
        {
            List<Member> memberList = new List<Member>();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand com = new SqlCommand("", con);
            com.CommandText = "SELECT RelationId,ref,[Type],[Role],sort FROM RelationInfoAram where RelationId='" + relationId + "' order by sort";
            SqlDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Member tempMember = new Member();
                    tempMember.ReferenceId = reader["ref"].ToString();
                    tempMember.RelationId = relationId; //reader["relationId"].ToString();
                    tempMember.Type = GetMemberType(reader["Type"].ToString());
                    tempMember.Role = reader["Role"].ToString();
                    tempMember.order = int.Parse(reader["sort"].ToString(), CultureInfo.InvariantCulture);
                    memberList.Add(tempMember);
                }
                if (!reader.IsClosed)
                    reader.Close();
                con.Close();
                return memberList;
            }
            else
            {
                if (!reader.IsClosed)
                    reader.Close();
                con.Close();
                return null;
            }
        }
    }
}
