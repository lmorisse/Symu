﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Agent.Models.Templates;
using SymuEngine.Classes.Agent.Models.Templates.Communication;

#endregion

namespace SymuEngine.Classes.Agent.Models
{
    /// <summary>
    ///     List of all available agent templates
    /// </summary>
    /// <example>
    ///     Communication email, phone, ...
    ///     Human
    ///     ...
    /// </example>
    public class AgentTemplates
    {
        public SimpleHumanTemplate SimpleHuman { get; } = new SimpleHumanTemplate();
        public EmailTemplate Email { get; } = new EmailTemplate();
        public FaceToFaceTemplate FaceToFace { get; } = new FaceToFaceTemplate();
        public IrcTemplate Irc { get; } = new IrcTemplate();
        public MeetingTemplate Meeting { get; } = new MeetingTemplate();
        public PhoneTemplate Phone { get; } = new PhoneTemplate();
        public ViaPlatformTemplate Platform { get; } = new ViaPlatformTemplate();
    }
}