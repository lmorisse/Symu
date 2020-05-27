#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents.Models.Templates;
using Symu.Classes.Agents.Models.Templates.Communication;

#endregion

namespace Symu.Classes.Agents.Models
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
        public StandardAgentTemplate Standard { get; } = new StandardAgentTemplate();
        public SimpleHumanTemplate Human { get; } = new SimpleHumanTemplate();
        public EmailTemplate Email { get; } = new EmailTemplate();
        public FaceToFaceTemplate FaceToFace { get; } = new FaceToFaceTemplate();
        public IrcTemplate Irc { get; } = new IrcTemplate();
        public MeetingTemplate Meeting { get; } = new MeetingTemplate();
        public PhoneTemplate Phone { get; } = new PhoneTemplate();
        public ViaPlatformTemplate Platform { get; } = new ViaPlatformTemplate();
        public InternetAccessTemplate Internet { get; } = new InternetAccessTemplate();
    }
}