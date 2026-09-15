using UnityEngine;
using System.Collections.Generic;
using CandyCrush.Core;
using CandyCrush.Events;
using CandyCrush.Network.Data;

namespace CandyCrush.Gameplay
{
    /// <summary>
    /// Manages team creation, membership, and collaborative tasks
    /// </summary>
    public class TeamManager : Singleton<TeamManager>
    {
        private TeamData currentTeam;
        private List<string> teamMembers = new List<string>();
        private Dictionary<string, int> memberContributions = new Dictionary<string, int>();

        public TeamData CurrentTeam => currentTeam;
        public List<string> TeamMembers => teamMembers;

        /// <summary>
        /// Create a new team
        /// </summary>
        public void CreateTeam(string teamName, string leaderId)
        {
            currentTeam = new TeamData(teamName, leaderId);
            teamMembers.Clear();
            teamMembers.Add(leaderId);
            memberContributions.Clear();
            memberContributions[leaderId] = 0;

            Debug.Log($"Team created: {teamName}");
            GameEvents.InvokeTeamCreated(currentTeam.TeamId);
        }

        /// <summary>
        /// Join an existing team
        /// </summary>
        public void JoinTeam(TeamData team, string memberId)
        {
            currentTeam = team;
            if (!teamMembers.Contains(memberId))
            {
                teamMembers.Add(memberId);
                memberContributions[memberId] = 0;
                currentTeam.MemberCount++;
            }

            Debug.Log($"Joined team: {team.Name}");
            GameEvents.InvokeTeamJoined(team.TeamId);
        }

        /// <summary>
        /// Add contribution to team task
        /// </summary>
        public void AddTeamContribution(string memberId, int amount)
        {
            if (memberContributions.ContainsKey(memberId))
            {
                memberContributions[memberId] += amount;
                currentTeam.TotalScore += amount;
                currentTeam.WeeklyScore += amount;

                // Check for milestones
                CheckTeamMilestones();
            }
        }

        /// <summary>
        /// Check if team reached milestone thresholds
        /// </summary>
        private void CheckTeamMilestones()
        {
            if (currentTeam == null)
                return;

            // TODO: Implement milestone checking based on team tasks
            // Milestones: 25%, 50%, 100%
            // Each milestone triggers rewards for all team members
        }

        /// <summary>
        /// Send message in team chat
        /// </summary>
        public void SendTeamMessage(string senderId, string message)
        {
            Debug.Log($"Team Message [{senderId}]: {message}");
            GameEvents.InvokeTeamMessageReceived(currentTeam.TeamId, message);
        }

        /// <summary>
        /// Get member contribution
        /// </summary>
        public int GetMemberContribution(string memberId)
        {
            if (memberContributions.ContainsKey(memberId))
                return memberContributions[memberId];
            return 0;
        }

        /// <summary>
        /// Get all team member stats
        /// </summary>
        public Dictionary<string, int> GetAllMemberStats()
        {
            return new Dictionary<string, int>(memberContributions);
        }
    }
}
