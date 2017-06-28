﻿using System;
using System.Globalization;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using Ploeh.AutoFixture.Kernel;

namespace Ploeh.AutoFixture.AutoNSubstitute
{
    /// <summary>
    /// Relays a request for an interface or an abstract class to a request for a substitute of that type.
    /// </summary>
    /// <remarks>
    /// This class serves as a residue collector, catching unanswered requests for an instance of an abstract type and 
    /// converting them to requests for a substitute of that type, dynamically generated by NSubstitute.
    /// </remarks>
    /// <seealso cref="IFixture.ResidueCollectors"/>
    public class SubstituteRelay : ISpecimenBuilder
    {
        /// <summary>
        /// Creates a substitute when request is an abstract type.
        /// </summary>
        /// <returns>
        /// A substitute resolved from the <paramref name="context"/> when <paramref name="request"/> is an abstract
        /// type or <see cref="NoSpecimen"/> for all other requests.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// An attempt to resolve a substitute from the <paramref name="context"/> returned an object that was not 
        /// created by NSubstitute.
        /// </exception>
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var requestedType = request as Type;
            if (requestedType == null || !requestedType.IsAbstract)
            {
#pragma warning disable 618
                return new NoSpecimen(request);
#pragma warning restore 618
            }

            object substitute = context.Resolve(new SubstituteRequest(requestedType));

            try
            {
                SubstitutionContext.Current.GetCallRouterFor(substitute);
            }
            catch (NotASubstituteException e)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Object resolved by request for substitute of {0} was not created by NSubstitute. " +
                        "Ensure that {1} was added to Fixture.Customizations.",
                        requestedType.FullName, typeof(SubstituteRequestHandler).FullName),
                    e);
            }

            return substitute;
        }
    }
}
