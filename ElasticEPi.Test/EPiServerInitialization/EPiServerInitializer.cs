using System;
using System.Configuration;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Hosting;
using InitializationModule = EPiServer.Framework.Initialization.InitializationModule;
using EPiServer.Globalization;

namespace ElasticEPi.Test.EPiServerInitialization
{
    public class EPiServerInitializer : IDisposable {
        public void Initialize(string applicationPath) {
            //LoadAllAssembliesInFolder(applicationPath);
            ApplyConfig();
            SetupHostingEnvironment(applicationPath);
            InitializationModule.FrameworkInitialization(HostType.TestFramework);
            ApplySiteDefinition();
            //CatalogRouteHelper.MapDefaultHierarchialRouter(RouteTable.Routes, false);
        }

        /// <summary>
        /// Loads all assemblies in specified folder.
        /// </summary>
        /// <param name="path">The folder path.</param>
        //private void LoadAllAssembliesInFolder(string path) {
        //    if (String.IsNullOrEmpty(path) || !Directory.Exists(path)) {
        //        return;
        //    }
        //    var loadedAssemblies = new HashSet<string>(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.FullName), StringComparer.OrdinalIgnoreCase);
        //    Parallel.ForEach(Directory.GetFileSystemEntries(path, "*.dll"), (file) => {
        //        var assemblyName = AssemblyName.GetAssemblyName(file);
                
        //        if (loadedAssemblies.Contains(assemblyName.FullName)) {
        //            return;
        //        }
        //        try {
        //            Assembly.Load(assemblyName);
        //        } catch (FileLoadException) {
        //        } catch (BadImageFormatException) {
        //        }
        //    });
        //}

        private void ApplyConfig() {
            var exeConfigurationFileMap = new ExeConfigurationFileMap { ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile };
            var openMappedExeConfiguration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            
            //ConfigurationSource.Instance = new FileConfigurationSource(openMappedExeConfiguration);
        }

        private void SetupHostingEnvironment(string applicationPath) {
            var hostingEnvironment = new MockHostingEnvironment {
                ApplicationVirtualPath = "/",
                ApplicationPhysicalPath = applicationPath
            };
            GenericHostingEnvironment.Instance = hostingEnvironment;
        }

        private void ApplySiteDefinition() {
            var siteDefinitionRepository = ServiceLocator.Current.GetInstance<SiteDefinitionRepository>();
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            SiteDefinition.Current = siteDefinitionRepository.List().FirstOrDefault();

            var startPage = contentLoader.Get<PageData>(SiteDefinition.Current.StartPage);

            ContentLanguage.Instance.SetCulture(startPage.MasterLanguage.TwoLetterISOLanguageName);

            

            //var siteName = "IntegrationTestSite";
            //var oldSiteDefinition = siteDefinitionRepository.Get(siteName);
            //if (oldSiteDefinition != null) {
            //    siteDefinitionRepository.Delete(oldSiteDefinition.Id);
            //}
            //var startPage = contentRepository.GetDefault<StartPage>(ContentReference.RootPage);
            //startPage.Name = "Start page";
            //var startPageReference = contentRepository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);
            //var siteDefinition = new SiteDefinition {
            //    Name = siteName,
            //    StartPage = startPageReference.ToReferenceWithoutVersion(),
            //    SiteUrl = new Uri("http://localhost/")
            //};
            //siteDefinition.Hosts.Add(new HostDefinition { Name = siteDefinition.SiteUrl.Authority });
            //siteDefinition.Hosts.Add(new HostDefinition { Name = SiteDefinition.WildcardHostName });
            //siteDefinitionRepository.Save(siteDefinition);
            //SiteDefinition.Current = siteDefinition;
        }

        public void Dispose() {
            
        }
    }
}