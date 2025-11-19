# SpinTrack API - Filter Operators Guide

Complete guide to all filter operators available in the SpinTrack API.

---

## 📋 Overview

The SpinTrack API supports **17 filter operators** for advanced querying capabilities. These operators allow you to build complex queries with precise filtering criteria.

---

## 🔤 String Operators (8 operators)

### 1. **Equals**
Exact match (case-sensitive)

```json
{
  "columnName": "Username",
  "operator": "Equals",
  "value": "johndoe"
}
```

### 2. **NotEquals**
Not equal to (case-sensitive)

```json
{
  "columnName": "Status",
  "operator": "NotEquals",
  "value": "Inactive"
}
```

### 3. **Contains**
String contains substring (case-sensitive)

```json
{
  "columnName": "Email",
  "operator": "Contains",
  "value": "@gmail.com"
}
```

### 4. **NotContains** ⭐ NEW
String does NOT contain substring (case-sensitive)

```json
{
  "columnName": "Email",
  "operator": "NotContains",
  "value": "test"
}
```

**Use Cases:**
- Exclude test accounts: `Email NotContains "test"`
- Exclude specific domains: `Email NotContains "@temp.com"`
- Filter out temporary data

### 5. **StartsWith**
String starts with prefix (case-sensitive)

```json
{
  "columnName": "Username",
  "operator": "StartsWith",
  "value": "admin"
}
```

### 6. **EndsWith**
String ends with suffix (case-sensitive)

```json
{
  "columnName": "Email",
  "operator": "EndsWith",
  "value": ".com"
}
```

### 7. **IsEmpty** ⭐ NEW
String is null or empty

```json
{
  "columnName": "MiddleName",
  "operator": "IsEmpty"
}
```

**Note:** No `value` field required. Checks for both `null` and empty string `""`.

**Use Cases:**
- Find users without middle name
- Find incomplete profiles
- Data quality checks

### 8. **IsNotEmpty** ⭐ NEW
String is not null and not empty

```json
{
  "columnName": "PhoneNumber",
  "operator": "IsNotEmpty"
}
```

**Note:** No `value` field required.

**Use Cases:**
- Find users with phone numbers
- Filter complete profiles
- Contact list generation

---

## 🔢 Numeric/Date Operators (5 operators)

### 9. **GreaterThan**
Value is greater than

```json
{
  "columnName": "DateOfBirth",
  "operator": "GreaterThan",
  "value": "1990-01-01"
}
```

**Applicable to:** Numbers (int, decimal, etc.), Dates (DateTime, DateOnly, DateTimeOffset)

### 10. **GreaterThanOrEqual**
Value is greater than or equal to

```json
{
  "columnName": "CreatedAt",
  "operator": "GreaterThanOrEqual",
  "value": "2024-01-01T00:00:00"
}
```

### 11. **LessThan**
Value is less than

```json
{
  "columnName": "DateOfBirth",
  "operator": "LessThan",
  "value": "2000-01-01"
}
```

### 12. **LessThanOrEqual**
Value is less than or equal to

```json
{
  "columnName": "CreatedAt",
  "operator": "LessThanOrEqual",
  "value": "2024-12-31T23:59:59"
}
```

### 13. **Between** ⭐ NEW
Value is between two values (inclusive)

```json
{
  "columnName": "DateOfBirth",
  "operator": "Between",
  "value": "1990-01-01",
  "valueTo": "1999-12-31"
}
```

**Note:** Requires both `value` (from) and `valueTo` (to) fields.

**Equivalent to:** `value >= from AND value <= to`

**Use Cases:**
- Date ranges: Birth date between 1990-1999
- Age ranges: Filter users born in specific decade
- Time periods: Created between two dates
- Numeric ranges: Salary between min and max

**Examples:**

**Age Range (Born in 1990s):**
```json
{
  "columnName": "DateOfBirth",
  "operator": "Between",
  "value": "1990-01-01",
  "valueTo": "1999-12-31"
}
```

**Recent Users (This Year):**
```json
{
  "columnName": "CreatedAt",
  "operator": "Between",
  "value": "2024-01-01T00:00:00",
  "valueTo": "2024-12-31T23:59:59"
}
```

---

## 📦 Collection Operators (2 operators)

### 14. **In**
Value is in the list

```json
{
  "columnName": "Status",
  "operator": "In",
  "values": ["Active", "Pending", "Suspended"]
}
```

**Note:** Uses `values` (array) instead of `value` (single string).

### 15. **NotIn**
Value is NOT in the list

```json
{
  "columnName": "Status",
  "operator": "NotIn",
  "values": ["Inactive", "Deleted"]
}
```

---

## ❓ Null Operators (2 operators)

### 16. **IsNull**
Value is null

```json
{
  "columnName": "MiddleName",
  "operator": "IsNull"
}
```

**Note:** No `value` field required.

### 17. **IsNotNull**
Value is not null

```json
{
  "columnName": "JobTitle",
  "operator": "IsNotNull"
}
```

---

## 📊 Operator Comparison Table

| Operator | Type | Requires Value | Requires ValueTo | Requires Values | Description |
|----------|------|----------------|------------------|-----------------|-------------|
| `Equals` | String | ✅ | ❌ | ❌ | Exact match |
| `NotEquals` | String | ✅ | ❌ | ❌ | Not equal |
| `Contains` | String | ✅ | ❌ | ❌ | Contains substring |
| `NotContains` ⭐ | String | ✅ | ❌ | ❌ | Does not contain |
| `StartsWith` | String | ✅ | ❌ | ❌ | Starts with prefix |
| `EndsWith` | String | ✅ | ❌ | ❌ | Ends with suffix |
| `IsEmpty` ⭐ | String | ❌ | ❌ | ❌ | Null or empty |
| `IsNotEmpty` ⭐ | String | ❌ | ❌ | ❌ | Not null and not empty |
| `GreaterThan` | Numeric/Date | ✅ | ❌ | ❌ | Greater than |
| `GreaterThanOrEqual` | Numeric/Date | ✅ | ❌ | ❌ | Greater or equal |
| `LessThan` | Numeric/Date | ✅ | ❌ | ❌ | Less than |
| `LessThanOrEqual` | Numeric/Date | ✅ | ❌ | ❌ | Less or equal |
| `Between` ⭐ | Numeric/Date | ✅ | ✅ | ❌ | Between range (inclusive) |
| `In` | Collection | ❌ | ❌ | ✅ | In list |
| `NotIn` | Collection | ❌ | ❌ | ✅ | Not in list |
| `IsNull` | Null | ❌ | ❌ | ❌ | Is null |
| `IsNotNull` | Null | ❌ | ❌ | ❌ | Is not null |

⭐ = New operator

---

## 💡 Real-World Examples

### Example 1: Find Active Users with Complete Profiles
```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "filters": [
    {
      "columnName": "Status",
      "operator": "Equals",
      "value": "Active"
    },
    {
      "columnName": "PhoneNumber",
      "operator": "IsNotEmpty"
    },
    {
      "columnName": "Email",
      "operator": "NotContains",
      "value": "test"
    }
  ]
}
```

### Example 2: Find Users Born in the 1990s
```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "filters": [
    {
      "columnName": "DateOfBirth",
      "operator": "Between",
      "value": "1990-01-01",
      "valueTo": "1999-12-31"
    }
  ]
}
```

### Example 3: Find Recently Created Non-Test Users
```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "filters": [
    {
      "columnName": "CreatedAt",
      "operator": "Between",
      "value": "2024-01-01T00:00:00",
      "valueTo": "2024-12-31T23:59:59"
    },
    {
      "columnName": "Email",
      "operator": "NotContains",
      "value": "test"
    },
    {
      "columnName": "Username",
      "operator": "NotContains",
      "value": "temp"
    }
  ]
}
```

### Example 4: Find Users Without Middle Name but With Phone
```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "filters": [
    {
      "columnName": "MiddleName",
      "operator": "IsEmpty"
    },
    {
      "columnName": "PhoneNumber",
      "operator": "IsNotEmpty"
    }
  ]
}
```

### Example 5: Complex Business Query
Filter active users from USA, born between 1985-1995, with complete contact info, excluding test accounts:

```json
{
  "pageNumber": 1,
  "pageSize": 20,
  "searchTerm": "",
  "filters": [
    {
      "columnName": "Status",
      "operator": "Equals",
      "value": "Active"
    },
    {
      "columnName": "Nationality",
      "operator": "Equals",
      "value": "USA"
    },
    {
      "columnName": "DateOfBirth",
      "operator": "Between",
      "value": "1985-01-01",
      "valueTo": "1995-12-31"
    },
    {
      "columnName": "PhoneNumber",
      "operator": "IsNotEmpty"
    },
    {
      "columnName": "Email",
      "operator": "NotContains",
      "value": "test"
    },
    {
      "columnName": "Email",
      "operator": "NotContains",
      "value": "temp"
    }
  ],
  "sortColumns": [
    {
      "columnName": "FirstName",
      "direction": "Ascending"
    }
  ]
}
```

---

## 🎯 Best Practices

### 1. **Use Between Instead of Two Comparisons**
❌ **Old Way:**
```json
[
  { "columnName": "DateOfBirth", "operator": "GreaterThanOrEqual", "value": "1990-01-01" },
  { "columnName": "DateOfBirth", "operator": "LessThanOrEqual", "value": "1999-12-31" }
]
```

✅ **Better Way:**
```json
[
  { "columnName": "DateOfBirth", "operator": "Between", "value": "1990-01-01", "valueTo": "1999-12-31" }
]
```

### 2. **Use IsEmpty/IsNotEmpty for String Checks**
❌ **Wrong Way:**
```json
{ "columnName": "PhoneNumber", "operator": "NotEquals", "value": "" }
```

✅ **Correct Way:**
```json
{ "columnName": "PhoneNumber", "operator": "IsNotEmpty" }
```

### 3. **Use NotContains for Exclusions**
❌ **Complex Way:**
```json
{ "columnName": "Email", "operator": "NotIn", "values": ["test@test.com", "temp@temp.com"] }
```

✅ **Simpler Way:**
```json
{ "columnName": "Email", "operator": "NotContains", "value": "test" }
```

### 4. **Combine Operators for Powerful Queries**
```json
{
  "filters": [
    { "columnName": "Status", "operator": "In", "values": ["Active", "Pending"] },
    { "columnName": "Email", "operator": "NotContains", "value": "test" },
    { "columnName": "PhoneNumber", "operator": "IsNotEmpty" },
    { "columnName": "DateOfBirth", "operator": "Between", "value": "1990-01-01", "valueTo": "1999-12-31" }
  ]
}
```

---

## 🔍 Operator Selection Guide

### For String Fields (Username, Email, FirstName, etc.)
- **Exact match:** `Equals`, `NotEquals`
- **Partial match:** `Contains`, `NotContains`, `StartsWith`, `EndsWith`
- **Empty check:** `IsEmpty`, `IsNotEmpty`
- **Null check:** `IsNull`, `IsNotNull`

### For Date Fields (DateOfBirth, CreatedAt, ModifiedAt)
- **Single comparison:** `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`
- **Range:** `Between` (recommended for date ranges)

### For Enum Fields (Status, Gender)
- **Single value:** `Equals`, `NotEquals`
- **Multiple values:** `In`, `NotIn`

### For Optional Fields (MiddleName, PhoneNumber, JobTitle)
- **Has value:** `IsNotNull`, `IsNotEmpty`
- **No value:** `IsNull`, `IsEmpty`

---

## ⚠️ Important Notes

1. **Case Sensitivity:** String operators are case-sensitive
2. **Date Format:** Use ISO 8601 format for dates: `YYYY-MM-DD` or `YYYY-MM-DDTHH:mm:ss`
3. **Null vs Empty:** 
   - `IsNull` checks for `null` only
   - `IsEmpty` checks for both `null` and empty string `""`
4. **Between is Inclusive:** The range includes both start and end values
5. **Multiple Filters:** All filters are combined with AND logic

---

## 📝 Validation Rules

- `value` is required for: Equals, NotEquals, Contains, NotContains, StartsWith, EndsWith, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual
- `value` and `valueTo` are required for: Between
- `values` array is required for: In, NotIn
- No parameters required for: IsNull, IsNotNull, IsEmpty, IsNotEmpty

---

## 🎉 Summary

**Total Operators: 17**

- **String Operators:** 8 (including 3 new ones)
- **Numeric/Date Operators:** 5 (including 1 new one)
- **Collection Operators:** 2
- **Null Operators:** 2

**New Operators Added:**
1. ⭐ `NotContains` - Exclude strings containing substring
2. ⭐ `IsEmpty` - Check for null or empty strings
3. ⭐ `IsNotEmpty` - Check for non-empty strings
4. ⭐ `Between` - Range queries for numbers and dates

These operators provide comprehensive filtering capabilities for building complex queries in the SpinTrack API!

---

**Last Updated:** 2024  
**API Version:** v1.0
