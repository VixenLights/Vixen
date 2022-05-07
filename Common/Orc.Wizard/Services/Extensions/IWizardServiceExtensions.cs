// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWizardServiceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;

    public static class IWizardServiceExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static Task<bool?> ShowWizardAsync<TWizard>(this IWizardService wizardService, object model = null)
            where TWizard : IWizard
        {
            Argument.IsNotNull(() => wizardService);

            var typeFactory = wizardService.GetTypeFactory();

            IWizard wizard = null;

            if (model is not null)
            {
                Log.Debug("Creating wizard '{0}' with model '{1}'", typeof(TWizard).GetSafeFullName(false), ObjectToStringHelper.ToFullTypeString(model));

                wizard = typeFactory.CreateInstanceWithParametersAndAutoCompletion<TWizard>(model);
            }
            else
            {
                Log.Debug("Creating wizard '{0}'", typeof(TWizard).GetSafeFullName(false));

                wizard = typeFactory.CreateInstance<TWizard>();
            }

            return wizardService.ShowWizardAsync(wizard);
        }
    }
}