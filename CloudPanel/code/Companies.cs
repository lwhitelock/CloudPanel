﻿using CloudPanel.ActiveDirectory;
using CloudPanel.Base.AD;
using CloudPanel.Base.Config;
using CloudPanel.Base.Database.Models;
using CloudPanel.Database.EntityFramework;
using CloudPanel.Rollback;
//
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
using System.Web;

namespace CloudPanel.code
{
    public class Companies
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Companies));

        private CloudPanelContext db = null;

        public Companies()
        {
            this.db = new CloudPanelContext(Settings.ConnectionString);
        }

        public void Create(Company newCompany, string domainName, string resellerCode)
        {
            OrganizationalUnits organizationalUnits = null;
            Groups groups = null;

            ReverseActions reverse = new ReverseActions();
            try
            {
                if (string.IsNullOrEmpty(domainName))
                    throw new MissingFieldException("", "DomainName");

                if (string.IsNullOrEmpty(resellerCode))
                    throw new MissingFieldException("", "ResellerCode");

                organizationalUnits = new OrganizationalUnits(Settings.Username, Settings.DecryptedPassword, Settings.PrimaryDC);
                groups = new Groups(Settings.Username, Settings.DecryptedPassword, Settings.PrimaryDC);

                // Check if the user provided a company code or not
                // If they didn't then we will automatically generate one
                if (string.IsNullOrEmpty(newCompany.CompanyCode))
                    newCompany.CompanyCode = OtherStatics.FindAvailableCompanyCode(newCompany.CompanyName, this.db);

                OrganizationalUnit newOrg = new OrganizationalUnit();
                newOrg.Name = newCompany.CompanyCode;
                newOrg.DisplayName = newCompany.CompanyName;
                newOrg.City = newCompany.City;
                newOrg.State = newCompany.State;
                newOrg.PostalCode = newCompany.ZipCode;
                newOrg.Country = newCompany.Country;
                newOrg.UPNSuffixes = new[] { domainName };
                newOrg.Description = newCompany.Description;

                var createdCompany = organizationalUnits.Create(Settings.HostingOU, newOrg);
                reverse.AddAction(Actions.CreateOrganizationalUnit, createdCompany.DistinguishedName);

                //
                // Create security groups
                //
                string strippedCompanyCode = newCompany.CompanyCode.Replace(" ", string.Empty);

                // Create Admins@ group
                SecurityGroup newGroup = new SecurityGroup();
                newGroup.Name = string.Format("Admins@", strippedCompanyCode);
                newGroup.SamAccountName = newGroup.Name.Length > 19 ? newGroup.Name.Substring(0, 18) : newGroup.Name;
                groups.Create(createdCompany.DistinguishedName, newGroup);
                reverse.AddAction(Actions.CreateSecurityGroup, newGroup.Name);

                // Create AllUsers@ group
                newGroup.Name = string.Format("AllUsers@", strippedCompanyCode);
                newGroup.SamAccountName = newGroup.Name.Length > 19 ? newGroup.Name.Substring(0, 18) : newGroup.Name;
                groups.Create(createdCompany.DistinguishedName, newGroup);
                reverse.AddAction(Actions.CreateSecurityGroup, newGroup.Name);

                // Create AllTSUsers@ group
                newGroup.Name = string.Format("AllTSUsers@", strippedCompanyCode);
                newGroup.SamAccountName = newGroup.Name.Length > 19 ? newGroup.Name.Substring(0, 18) : newGroup.Name;
                groups.Create(createdCompany.DistinguishedName, newGroup);
                reverse.AddAction(Actions.CreateSecurityGroup, newGroup.Name);
                groups.AddGroup("GPOAccess@" + resellerCode, newGroup.Name); // Add group to the GPOAccess group in resellers OU

                //
                // Create Exchange and Applications OU
                //
                newOrg = new OrganizationalUnit();
                newOrg.Name = Settings.ExchangeOU;
                newOrg.DisplayName = Settings.ExchangeOU;
                newOrg.UPNSuffixes = new[] { domainName };

                var createdOrg = organizationalUnits.Create(createdCompany.DistinguishedName, newOrg);
                reverse.AddAction(Actions.CreateOrganizationalUnit, createdOrg.DistinguishedName);

                newOrg = new OrganizationalUnit();
                newOrg.Name = Settings.ApplicationsOU;
                newOrg.DisplayName = Settings.ApplicationsOU;
                newOrg.UPNSuffixes = new[] { domainName };

                createdOrg = organizationalUnits.Create(createdCompany.DistinguishedName, newOrg);
                reverse.AddAction(Actions.CreateOrganizationalUnit, createdOrg.DistinguishedName);
                
                // Add to SQL
                log.DebugFormat("Saving new companyr {0} to the database.", newCompany.CompanyName);
                newCompany.Created = DateTime.Now;
                newCompany.DistinguishedName = createdCompany.DistinguishedName;
                newCompany.IsReseller = false;

                db.Companies.Add(newCompany);
                db.SaveChanges();

                log.InfoFormat("Successfully created new company {0}", newCompany.CompanyName);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error creating new company: {0}", ex.ToString());
                reverse.RollbackNow();
                throw;
            }
            finally
            {
                if (groups != null)
                    groups.Dispose();

                if (organizationalUnits != null)
                    organizationalUnits.Dispose();
            }
        }
    }
}