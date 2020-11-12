
# MagentoScanner
Magento Security Scanner is an open source tool that automates the process recon against a Magento CMS.
This is an early version under developpement, feel free to add your brick :) pull requets are welcomed !

**Usage**

    MagentoScanner [options] (e.g. MagentoScanner -u http://site.com/ --ignore--ssl --discover)
    
    Options:
      -u|--url            Target URL (e.g. 'http://www.site.com/')
      -ssl|--ignore-ssl   Ignore SSL errors
      -mr|--mage-report   Scan target using https://www.magereport.com/
      -d|--discover       Search for some default locations (admin panels, customer panels...)
      -ua|--user-agent    HTTP User-Agent header value
      -H|--headers        List of headers (e.g. "Accept-Language: fr|ETag: 123")
      -c|--cookie         HTTP cookie header
**Execution example**
## Response Headers 
![Response Headers ](https://github.com/soufianetahiri/MagentoScanner/blob/main/1.png?raw=true)
## SiteMap/Version/Patches/Admin Panel
![SiteMap/Version/Patches/Admin Panel ](https://github.com/soufianetahiri/MagentoScanner/blob/main/2.png?raw=true)
## Content Discovery
![Content Discovery ](https://github.com/soufianetahiri/MagentoScanner/blob/main/3.png?raw=true)

# Road map

 - [ ] Catalog Information
 - [ ] Installed modules Enumeration
 - [ ] Malware Scan
