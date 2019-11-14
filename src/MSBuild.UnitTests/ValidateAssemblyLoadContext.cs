﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if FEATURE_ASSEMBLYLOADCONTEXT

using Microsoft.Build.Framework;
using Microsoft.Build.Shared;
using Microsoft.Build.Utilities;
using System.Runtime.Loader;

namespace Microsoft.Build.UnitTests
{
    public class ValidateAssemblyLoadContext : Task
    {
        public override bool Execute()
        {
            var thisLoadContext = AssemblyLoadContext.GetLoadContext(typeof(ValidateAssemblyLoadContext).Assembly);

            // Check by name because this always reports false, presumably due to ALC shenanigans:
            // if (thisLoadContext is MSBuildLoadContext context)
            if (thisLoadContext.GetType().FullName == typeof(MSBuildLoadContext).FullName)
            {
#if NETCOREAPP && !NETCOREAPP2_1 // TODO: enable this functionality when targeting .NET Core 3.0+
                if (!thisLoadContext.Name.EndsWith(typeof(ValidateAssemblyLoadContext).Assembly.GetName().Name + ".dll"))
                {
                    Log.LogError($"Unexpected AssemblyLoadContext name: \"{thisLoadContext.Name}\", but the current executing assembly was {typeof(ValidateAssemblyLoadContext).Assembly.GetName().Name}");
                }
                else
                {
                    Log.LogMessage(MessageImportance.High, $"Task {nameof(ValidateAssemblyLoadContext)} loaded in AssemblyLoadContext named {thisLoadContext.Name}");
                }
#endif
            }
            else
            {
                Log.LogError($"Load context was a {thisLoadContext.GetType().AssemblyQualifiedName} instead of an {typeof(MSBuildLoadContext).AssemblyQualifiedName}");
            }

            return !Log.HasLoggedErrors;
        }
    }
}
#endif
