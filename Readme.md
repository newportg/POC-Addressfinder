# International Address Management

This project provides a comprehensive solution for managing international addresses. It includes features for validating, formatting, and storing addresses from various countries around the world.

## Which International Standard should you use ...?
The probalem with international standards is that they are tailored to specific use cases and may not cover all the nuances of address formats across different countries. For example, the Universal Postal Union (UPU) provides guidelines for international mail addressing, but it may not account for local variations in address formats. Similarly, the ISO 19160 standard focuses on geospatial information and may not be suitable for all address management needs.

# ISO 19160 or ISO 20022 ?
ISO cannot make up there mind.

ISO 20022 and ISO 19160 (UPU S42) serve different, though related, purposes: ISO 20022 defines a standardised, structured data format for financial messaging, including specific, granular fields for address components, primarily for efficiency and accuracy in financial transactions. In contrast, UPU S42 is a global postal addressing standard that focuses on the logical structure of postal addresses and provides country-specific templates to guide mailers and postal systems in accurately formatting addresses for efficient cross-border mail processing.

### Key Differences 

*   **Scope:** 
    ISO 20022 is a financial messaging standard, while UPU S42 is a postal addressing standard. 
*   **Focus:** 
    ISO 20022 emphasises structured data for financial transactions, whereas S42 provides a flexible framework to format addresses for mail delivery. 
*   **Application:** 
    ISO 20022 is critical for financial operations and regulatory compliance, while UPU S42 supports postal operations and communication.

### ISO 19160 - UPU S42
*   **Purpose:** 
  To provide a universal framework for postal addressing that can be adapted by any country to standardise the format of their national addresses, thereby streamlining cross-border mail. 
*   **Structure:** 
  Consists of a generic list of address elements and country-specific templates that guide users on how to map these elements into a correctly formatted address. 
*   **Key Feature:** 
  Aims for easy adaptation by web applications and promotes efficient mail processing and data verification at national and international levels. 
*   **Context:** 
   Developed by the Universal Postal Union (UPU) for postal services to improve mail delivery and address management.

### UPU Universal Postal Union S42 Addressing Standard
The UPU S42 standard provides a comprehensive framework for postal addressing, designed to accommodate the diverse formats used by different countries. It includes a generic list of address elements and country-specific templates to guide users in formatting addresses correctly for mail delivery.

| **Element** | **Field name** |
|----|----|
| Organisation | Organisation Name                 |
|              | Department Name                   |
| Premises     | Sub Building Name                 |
|              | Building Name                     |
|              | Building Number                   |
| Thoroughfare | Dependent Thoroughfare Name       |
|              | Dependent Thoroughfare Descriptor |
|              | Thoroughfare Name                 |
|              | Thoroughfare Descriptor           |
| Locality     | Double Dependent Locality         |
|              | Dependent Locality                |
|              | Post town                         |
| Postcode     | Postcode                          |
| PO Box       | PO Box                            |
| Country Name |                                   |
| ISO3166-2    | 2 character code                  | 
| ISO3166-3    | 3 character code                  |
| ISO3166-N    | Numeric code                      |

## ISO 20022
*   **Purpose:** 
    To standardise the capture and exchange of financial transaction data, ensuring consistency and reducing errors in payments and other financial services. 
*   **Structure:** 
    Uses a highly structured format with discrete data elements (like street name, postal code, town name) defined within XML tags. 
*   **Key Feature:** 
    Breaks down addresses into granular, machine-readable components, facilitating automated processing, data quality improvements, and compliance with regulations. 
*   **Evolution:** 
    The standard has evolved, with the 2019 version (PostalAddress24) offering a more detailed 15-field structure compared to the 2009 version (PostalAddress6). 
*   **Context:** 
    Primarily used by financial institutions and payment systems (e.g., SWIFT) for cross-border and high-value payments.
* **Important : The ISO 20022 standard currently only applies to 32 countries.**

### Key ISO 20022 Address Schema Elements

* **Department**: (<Dept>): For specific business units or departments. 
* **Sub Department**: (<SubDept>): The sub department name.
* **Street Name**: (<StrtNm>): The name of the street. 
* **Building Number**: (<BldgNb>): The number of the building. 
* **Building Name**: (<BldgNm>): The name of the building.
* **Floor**: (<Flr>) : The building floor.
* **Post Box Number**: <PstBx>) : The PostBox.
* **Room**: (<Room>) : Room number or name.
* **Postal Code**: (<PstCd>): The postal code for the address. 
* **Town Name**: (<TwnNm>): The name of the city or town. 
* **Town Location Name**: (<TwnLctnNm>): A specific location within a town, like a district. 
* **District Name**: (<DstrctNm>): The name of a larger district within a country. 
* **Country Subdivision**: (<CtrySubDvsn>): A state, province, or other governmental subdivision. 
* **Country**: (<Ctry>): A two-character ISO country code (e.g., US, GB). 
* **Address Line**: (<AdrLine>): An optional line for additional address information.

### References
* [ISO 20022: Understanding the Impact for Global Address Data | GB](https://www.loqate.com/en-gb/blog/iso-20022/)
* [ISO-20022-address-format-guide](https://www.postgrid.com/iso-20022-address-format-guide)
* [Structured Postal Address](https://www.swift.com/swift-resource/252113/download)
* [Best Practices for Storing Addresses : The Essential Guide](https://www.geopostcodes.com/blog/best-practices-storing-addresses/)


# Conclusion 
By far the best standard to use is the UPU S42 standard as it is more flexible and can be adapted to a wider range of use cases. The ISO 20022 standard is more rigid and may not be suitable for all address management needs, especially for non-financial applications. The UPU S42 standard provides a comprehensive framework for postal addressing that can be easily adapted by web applications, making it a more practical choice for most address management scenarios.

The UPU standard has more elements than is used by many countries, for instance the UK as shown below, only uses 9 of the 15 elements defined by the UPU S42 standard. This is because the UPU S42 standard is designed to be flexible and adaptable to the specific needs of each country, allowing them to choose which elements are relevant for their postal addressing system.

## UK Address Format, as described by the UPU

|United Kingdom GBR, GB ||
|--|--|
|Line 1 | Organization 
|Line 2 | Department |
|Line 3 | PostBox |
|Line 4 | SubBuilding Building |
|Line 5 | Premise Thoroughfare |
|Line 6 | DoubleDependentLocality |
|Line 7 | DependentLocality |
|Line 8 | Locality |
|Line 9 | PostalCode|

# Application of the UPU S42 Standard

* GUI
  * Display a dropdown of Country flags and tooltip of Country names.
  * Display a input field for the user to enter an address.
  * On completion of the Address entry, the address and selected country should be sent to the API for processing and the results displayed on the GUI.
  * The returned processed addrress should be displayed in the correct format for the selected country, as defined by the UPU S42 standard. For example, if the user selects the United Kingdom, the address should be displayed in the format defined by the UPU S42 standard for the United Kingdom, which includes elements such as Organization, Department, PostBox, SubBuilding Building, Premise Thoroughfare, DoubleDependentLocality, DependentLocality, Locality and PostalCode.


* API's 
  * A API which returns a list of Countries, their flags and formats.
  * A API which takes an Country and Address entered as a single line, and returns the address elements for that country, as defined by the UPU S42 standard. For example, if the user selects the United Kingdom and enters an address, the API should return the address elements for the United Kingdom, such as Organization, Department, PostBox, SubBuilding Building, Premise Thoroughfare, DoubleDependentLocality, DependentLocality, Locality and PostalCode.


So for each country use the [Address Formats JSON](./AddressFormats.json) file to determine which elements to use for that country.

As a reference use the LOQATE Batch interface which will return the address elements for a given address, as shown below. 

```
curl --request POST \
  --url https://api.addressy.com/Cleansing/International/Batch/v1.10/json6.ws \
  --header 'Content-Type: application/json' \
  --data '{
  "Key": "UM22-TE99-HT99-UB71",
  "GeoCode": false,
  "Options": {
    "Process": "Verify",
    "Enhance": false
  },
  "Addresses": [
    {
      "Department": "",
      "PostalCode": "tf26rf",
      "Country": "United Kingdom",
      "Address": "93 teagues crescent"
    }
  ]
}
```
```
{  
"Address": "93 Teagues Crescent<br>Trench<br>Telford<br>TF2 6RF",  
"Address1": "93 Teagues Crescent",  
"Address2": "Trench",  
"Address3": "Telford",  
"Address4": "TF2 6RF",  
"DeliveryAddress": "93 Teagues Crescent<br>Trench",  
"DeliveryAddress1": "93 Teagues Crescent",  
"DeliveryAddress2": "Trench",  
"AdministrativeArea": "Shropshire",  
"Locality": "Telford",  
"DependentLocality": "Trench",  
"Thoroughfare": "Teagues Crescent",  
"Premise": "93",  
"PostalCode": "TF2 6RF",  
"CountryName": "United Kingdom",  
"ISO3166-2": "GB",  
"ISO3166-3": "GBR",  
"ISO3166-N": "826",  
"PostalCodePrimary": "TF2 6RF",  
"AVC": "V44-I44-P6-100",  
"AQI": "A",  
"Sequence": "1",  
"MatchRuleLabel": "Rlfnp",  
"HyphenClass": "B",  
"PremiseNumber": "93",  
"Country": "GB"  
}
```
