#region Licence

// Description: Symu - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Symu.Classes.Agents.Models.CognitiveModel;
using Symu.Classes.Scenario;
using Symu.Environment;
using Symu.Forms;
using Symu.Repository.Networks.Knowledges;
using SymuGroupAndInteraction.Classes;

#endregion

namespace SymuGroupAndInteraction
{
    public partial class Home : SymuForm
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();

        public Home()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            DisplayButtons();

            #region Environment

            tbWorkers.Text = _environment.WorkersCount.ToString(CultureInfo.InvariantCulture);
            GroupsCount.Text = _environment.GroupsCount.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region sphere of interaction

            RadomlyGenerated.Checked = true;
            AllowNewInteractions.Checked = true;
            MaxSphereDensity.Text =
                OrganizationEntity.Models.InteractionSphere.MaxSphereDensity.ToString();
            MinSphereDensity.Text =
                OrganizationEntity.Models.InteractionSphere.MinSphereDensity.ToString();
            WeightKnowledge.Text = OrganizationEntity.Models.InteractionSphere.RelativeKnowledgeWeight.ToString();
            WeightActivities.Text = OrganizationEntity.Models.InteractionSphere.RelativeActivityWeight.ToString();
            WeightBeliefs.Text = OrganizationEntity.Models.InteractionSphere.RelativeBeliefWeight.ToString();
            WeightGroups.Text = OrganizationEntity.Models.InteractionSphere.SocialDemographicWeight.ToString();

            #endregion

            #region Interaction

            OrganizationEntity.Models.InteractionSphere.SetInteractionPatterns(InteractionStrategy.Homophily);
            OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = true;
            OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 0.1F;
            ThresholdFornewInteraction.Text = OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns
                .ThresholdForNewInteraction.ToString();
            MaxDailyInteractions.Text = "2";

            Homophily.Checked = true;

            #endregion

            #region Knowledge

            KnowledgeRandom.Checked = true;
            cbInitialKnowledgeLevel.Items.AddRange(KnowledgeLevelService.GetNames());
            cbInitialKnowledgeLevel.SelectedItem = KnowledgeLevelService.GetName(_environment.KnowledgeLevel);

            #endregion

            #region Activities

            ActivitiesRandom.Checked = true;

            #endregion
        }

        protected override void UpdateSettings()
        {
            #region Knowledge

            if (KnowledgeSame.Checked)
            {
                _environment.Knowledge = 0;
            }

            if (KnowledgeByGroup.Checked)
            {
                _environment.Knowledge = 1;
            }

            if (KnowledgeRandom.Checked)
            {
                _environment.Knowledge = 2;
            }

            _environment.KnowledgeLevel =
                KnowledgeLevelService.GetValue(cbInitialKnowledgeLevel.SelectedItem.ToString());

            #endregion

            #region sphere of interaction

            OrganizationEntity.Models.InteractionSphere.RandomlyGeneratedSphere = RadomlyGenerated.Checked;

            #endregion

            #region Activities

            if (ActivitiesSame.Checked)
            {
                _environment.Activities = 0;
            }

            if (ActivitiesByGroup.Checked)
            {
                _environment.Activities = 1;
            }

            if (ActivitiesRandom.Checked)
            {
                _environment.Activities = 2;
            }

            #endregion

            #region Interactions

            OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions =
                AllowNewInteractions.Checked;

            #endregion

            var scenario = new TimeBasedScenario(_environment)
            {
                NumberOfSteps = ushort.Parse(tbSteps.Text)
            };

            AddScenario(scenario);

            SetTimeStepType(TimeStepType.Daily);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            DisplayButtons();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Start(_environment);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        public override void DisplayStep()
        {
            DisplayButtons();
            WriteTextSafe(TimeStep, _environment.Schedule.Step.ToString());
            UpDateMessages();
            UpdateAgents();
        }

        private void UpDateMessages()
        {
            WriteTextSafe(lblMessagesSent, _environment.Messages.SentMessagesCount.ToString());
            var notAcceptedMessages = _environment.WhitePages.AllAgents()
                .Sum(agent => agent.MessageProcessor.NotAcceptedMessages.Count);
            WriteTextSafe(NotAcceptedMessages, notAcceptedMessages.ToString());
        }

        private void UpdateAgents()
        {
            WriteTextSafe(Triads,
                _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(InteractionSphereCount,
                _environment.IterationResult.OrganizationFlexibility.Links.Last().Density
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(SphereDensity,
                _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density
                    .ToString("F1", CultureInfo.InvariantCulture));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Resume();
        }

        private void DisplayButtons()
        {
            DisplayButtons(btnStart, btnStop, btnPause, btnResume);
        }

        private void tbWorkers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.WorkersCount = byte.Parse(tbWorkers.Text);
                tbWorkers.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbWorkers.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbWorkers.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void GroupsCount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.GroupsCount = byte.Parse(GroupsCount.Text);
                GroupsCount.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                GroupsCount.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                GroupsCount.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void Coworkers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.InteractionsBasedOnActivities =
                    float.Parse(InteractionActivities.Text);
                InteractionActivities.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InteractionActivities.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InteractionActivities.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void DeliberateSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.InteractionsBasedOnKnowledge =
                    float.Parse(InteractionKnowledge.Text);
                InteractionKnowledge.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InteractionKnowledge.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InteractionKnowledge.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void ThresholdForNewInteraction_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction =
                    float.Parse(ThresholdFornewInteraction.Text);
                ThresholdFornewInteraction.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                ThresholdFornewInteraction.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                ThresholdFornewInteraction.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void DailyInteractions_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions =
                    byte.Parse(MaxDailyInteractions.Text);
                MaxDailyInteractions.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MaxDailyInteractions.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MaxDailyInteractions.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void InteractionSocialDemographics_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.InteractionsBasedOnSocialDemographics =
                    float.Parse(InteractionSocialDemographics.Text);
                InteractionSocialDemographics.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InteractionSocialDemographics.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InteractionSocialDemographics.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void InteractionBeliefs_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.InteractionsBasedOnBeliefs =
                    float.Parse(InteractionBeliefs.Text);
                InteractionBeliefs.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InteractionBeliefs.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InteractionBeliefs.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MinSphereDensity_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.InteractionSphere.MinSphereDensity = float.Parse(MinSphereDensity.Text);
                MinSphereDensity.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MinSphereDensity.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MinSphereDensity.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MaxSphereDensity_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.InteractionSphere.MaxSphereDensity = float.Parse(MaxSphereDensity.Text);
                MaxSphereDensity.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MaxSphereDensity.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MaxSphereDensity.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.InteractionSphere.RelativeKnowledgeWeight = float.Parse(WeightKnowledge.Text);
                WeightKnowledge.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                WeightKnowledge.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                WeightKnowledge.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void WeightActivities_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.InteractionSphere.RelativeActivityWeight = float.Parse(WeightActivities.Text);
                WeightActivities.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                WeightActivities.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                WeightActivities.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void WeightGroups_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.InteractionSphere.SocialDemographicWeight = float.Parse(WeightGroups.Text);
                WeightGroups.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                WeightGroups.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                WeightGroups.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void WeightBeliefs_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.InteractionSphere.RelativeBeliefWeight = float.Parse(WeightBeliefs.Text);
                WeightBeliefs.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                WeightBeliefs.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                WeightBeliefs.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void Homophily_CheckedChanged(object sender, EventArgs e)
        {
            if (!Homophily.Checked)
            {
                return;
            }

            OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.SetInteractionPatterns(
                InteractionStrategy.Homophily);
            InteractionActivities.Text = "0";
            InteractionBeliefs.Text = "0";
            InteractionKnowledge.Text = "0";
            InteractionSocialDemographics.Text = "0";
        }

        #region Menu

        private void symuorgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://symu.org");
        }

        private void documentationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("http://docs.symu.org/");
        }

        private void sourceCodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("http://github.symu.org/");
        }

        private void issuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://github.symu.org/issues");
        }

        #endregion
    }
}