---
title: "How to use the DrillString microservice?"
output: html_document
---

Typical Usage
===
1. Upload a new MyParentData using the `Post` web api method.
2. Call the `Get` method with the identifier of the uploaded MyParentData as argument. 
The return Json object contains the MyParentData description.
3. Optionally send a `Delete` request with the identifier of the MyParentData in order to delete the MyParentData if you do not 
want to keep the MyParentData uploaded on the microservice.


