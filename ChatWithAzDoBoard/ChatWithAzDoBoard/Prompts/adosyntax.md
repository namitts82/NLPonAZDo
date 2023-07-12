
# Run a semantic work item search in Azure Boards and Azure DevOps

<a id="search-box"/>

You can find work items by using shortcut filters or by specifying keywords or phrases. You can also use specific fields/field values, assignment or date modifications, or using Equals, Contains, and Not operators. Searching isn't case-sensitive. Use semantic searches when you want to do the following tasks:

- Find a specific work item using its ID or a keyword
- Find one or more work items across all projects in a fast, flexible manner
- Run a full text search across all work item fields
- Review work items assigned to a specific team member
- Search against specific work item fields to quickly narrow down a list of work items
- Determine what key words support a managed search

You can run a powerful [semantic search](#start-search) from the web portal for Azure DevOps Services or for on-premises deployments when the [server instance has been configured with the work item search extension](../../project/search/get-started-search.md). 

 

<a name:"start-search"></a>

## Start a semantic search for work items

With semantic search you can search: 
- Across one or more projects  
- Across all work item fields using free text  
- Against specific work item fields  

Free text search easily searches across all work item fields, including custom fields, which enables more natural searches. Search results are displayed in a snippet view where the search matches found are highlighted. Semantic search also integrates with work item tracking, providing familiar controls to view, edit, comment, and share information within a work item form. 


### Fine-tune semantic search results 

1. Fine-tune your search by specifying the fields to search. Enter `a:` and a user name
   to search for all items assigned to that user.


   The quick filters you can use are:

   * `a:` for **Assigned to:** 
   * `c:` for **Created by:** 
   * `s:` for **State** 
   * `t:` for **Work item type**<p />

2. Start entering the name of a field in your work items; for example, type `ta`.

   The dropdown list shows work item field name suggestions 
   that match user input and help the user to complete the search faster. For example, a search such as 
   `tags:Critical` finds all work items tagged "Critical". 

3. Add more filters to further narrow your search, and use Boolean operators
   to combine terms if necessary. For example, 
   `a: Chris t: Bug s: Active` finds all active bugs assigned
   to a user named "Chris".

4. Narrow your search to specific types
   and states, by using the drop-down selector lists at the top of the results page.


1. Fine-tune your search by specifying the fields to search. Enter `a:` and a user name
   to search for all items assigned to that user.


   The quick filters you can use are:

   * `a:` for **Assigned to:** 
   * `c:` for **Created by:** 
   * `s:` for **State** 
   * `t:` for **Work item type**<p />

supported work item types are: Epic, Feature, Issue, Task, Testcase, User Story and Bug
supported States are: New, Active, Resolved, Closed, and Removed

2. Start entering the name of a field in your work items; for example, type `ta`.

   The dropdown list shows work item field name suggestions 
   that match user input and help the user to complete the search faster. For example, a search such as 
   `tags:Critical` finds all work items tagged "Critical". 

3. Add more filters to further narrow your search, and use Boolean operators
   to combine terms if necessary. For example, 
   `a: Chris t: Bug s: Active` finds all active bugs assigned
   to a user named "Chris".

4. Narrow your search to specific types
   and states, by using the drop-down selector lists at the top of the results page.


## Find items based on keywords or phrases

Keywords or phrases that you type into the search box return a list of work items that contain those keywords or phrases in the **Description**, **Repro Steps**, or **Title** fields. Enclose each phrase in quotation marks.

In the **Search work items** box, type a keyword or phrase that appears in the **Title**, **Description**, or **Repro Steps** fields for the work items of interest.

Enclose multiple words in quotation marks.

For example, to find work items with the specified keywords in the **Title** or **Description** fields:

-   For the keyword "duplication", enter **duplication**.  
-   For the phrase "Getting Started", enter **"Getting Started"**.  
-   For the phrase "Getting Started" or the keyword "feature", enter **feature "Getting Started"**.

|Filter for items that contain these keywords or phrases:|Enter the following string:|
|---|---|
|Duplication|`duplication`|
|Getting Started|`"Getting Started"`|
|Feature and Getting Started|`feature "Getting Started"`|

You can run partial or exact match queries on a keyword or a phrase contained within any text field. Or, you can run a full-text search query by filtering on keywords and phrases contained within the full-text search index. Team Foundation automatically indexes all long-text fields with a data type of **PlainText** and **HTML** and the **Title** field for full-text search.

## Find items based on specific fields and field values

To find work items based on a keyword or phrase contained within other text string fields, specify either the friendly name or the reference name of the field. Enclose each phrase in quotation marks. You can determine the friendly name of a field by hovering over the field within a work item form. To determine the reference name of commonly used fields or to find a field that isn't listed on the form, see [Work item field index](../work-items/guidance/work-item-field.md).

|Filter for items that meet this criteria:|Enter the following string:|  
|---|---|  
|Contains one attached file.|`System.AttachedFileCount:1`|  
|Cut user stories.|`T:Story Reason:Cut`<br/>|  
|Resolved by Peter.|`"Resolved By":Peter` <br/>Or<br/>`Microsoft.VSTS.Common.ResolvedBy:Peter` |  
|Modified today.|`"Changed Date":@Today`|  
|Created yesterday as a test activity.|`"Created Date":@Today-1 Activity:Test`|  

> [!NOTE]     
> Some fields, such as **History** and **Description**, do not support partial word text searches. For example, if the **History** field contains the phrase `reproducible behavior` and you search for `History:repro` the work item isn't found. However, if you search for the complete string `History:reproducible` the work item is found.

## Use @Me or @Today macros

The <strong>@Me</strong> macro expands to the full name of the current user in any work item search. The <strong>@Me</strong> macro is especially useful for creating a search that you can share with other users, and it can simplify your work by reducing the number of characters you must type to specify your own user name. For a description of all macros, see [Query fields, operators, and macros, Query macros or variables](query-operators-variables.md#macros). 



---
:::row:::
   :::column span="2":::
      **Filter for**
   :::column-end::: 
   :::column span="2":::
      **Enter the following string**
   :::column-end:::
:::row-end:::
---
:::row:::
   :::column span="2":::
      Currently assigned to you
   :::column-end::: 
   :::column span="2":::
      `A:@Me`
   :::column-end:::
:::row-end:::
---
:::row:::
   :::column span="2":::
      Created by you
   :::column-end::: 
   :::column span="2":::
      `C:@Me`
   :::column-end:::
:::row-end:::
---
:::row:::
   :::column span="2":::
      Resolved yesterday
   :::column-end::: 
   :::column span="2":::
      `Resolved Date:@Today-1`
   :::column-end:::
:::row-end:::
---
:::row:::
   :::column span="2":::
      Modified seven days ago
   :::column-end::: 
   :::column span="2":::
      `System.ChangedDate:@Today-7`
   :::column-end:::
:::row-end:::
---
:::row:::
   :::column span="2":::
      Created yesterday under the Phone Saver team
   :::column-end::: 
   :::column span="2":::
      `Created Date:@Today-1 And Area Path:FabrikamFiber\Phone Saver`
   :::column-end:::
:::row-end:::
---

## Use Equals, Contains, and Not operators

Use the following search operators to specify search criteria:

**=** (EQUALS) to search for exact matches of text.  
**:** (CONTAINS) to search for partial matches of text.  
**-** (NOT) to exclude work items that contain certain text. The **NOT** operator can only be used with field names.

The following examples show how to use operators when you create a search string.  

|Filter for items that meet this criteria:|Enter the following string:|  
|-----------------------------------------|--------------------------|
|Assigned to Peter and not Active.|`A:Peter -S:Active`|
|In which the Activity field wasn't `Development`.|`- Activity:Development`|
|Resolved by Peter.|`"Resolved By":Peter`|
|Contain the keyword `triage` in the title or description, aren't assigned to you, and aren't closed.|`triage -A:@me -S:Closed`|
|Active bugs that are assigned to you that don't contain the keyword `bugbash` in the title.|`S:Active T:bug A:@Me -Title:bugbash`


Q1: create search syntax for the query "find the number of active open ai engagements"
A1: T:Engagement S:Active OpenAI

Q1: create search syntax for the query "find the number of Industry metaverse engagements"
A1: T:Engagement Metaverse 

Q1: create search syntax for the query "Are industry metaverse engagements using openai?"
A1: T:Engagement Metaverse OpenAI

Q1: create search syntax for the query "What are the technology trends in Open Ai engagements"
A1:  T:Engagement OpenAI 

Q1: create search syntax for the query "What are the technology trends in Open Ai resource request"
A1:  T:Resource Request Open AI

Q1: create search syntax for the query "What are the technology trends in Open Ai project"
A1:   T:Engagement Open AI

Q1: create search syntax for the query "What are the technology trends in IMV project"
A1:  T:Engagement Metaverse 

Q1: create search syntax for the query "Find all Active Open AI engagements assigned to namts"
A1: T:Engagement S:Active OpenAI A:namts

Q1: create search syntax for the query "Find all Open AI engagements assigned to me"
A1: T:Engagement OpenAI A:@Me

Q1: create search syntax for the query "Find all projects with keywords OpenAi and IMV"
A1: OpenAI AND IMV 

Q1: create search syntax for the query "Find all projects with keywords OpenAi or IMV"
A1: OpenAI OR IMV 

Q1: create search syntax for the query "Find all projects with Tagged with IMV"
A1: tags:IMV

Q1: {0}
A1:
