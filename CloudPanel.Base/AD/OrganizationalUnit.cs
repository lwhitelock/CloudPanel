﻿//
// Copyright (c) 2014, Jacob Dixon
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. All advertising materials mentioning features or use of this software
//    must display the following acknowledgement:
//    This product includes software developed by KnowMoreIT and Compsys.
// 4. Neither the name of KnowMoreIT and Compsys nor the
//    names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY Jacob Dixon ''AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL Jacob Dixon BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Base.AD
{
    public class OrganizationalUnit
    {
        public Guid ObjectGUID { get; set; }

        public string DistinguishedName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string WhenChanged { get; set; }

        public string WhenCreated { get; set; }

        public string AdminDisplayName { get; set; }

        public string AdminDescription { get; set; }
        
        /// <summary>
        /// Property: street
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Property: l
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Property: st
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Property: postalCode
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Country spelled out
        /// Property: co
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Two digit country code
        /// Property: c
        /// </summary>
        public string CountryCode { get; set; }

        public string[] UPNSuffixes { get; set; }
    }
}
