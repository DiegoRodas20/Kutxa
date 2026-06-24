# Explicación de los tests SQL — Prueba técnica

Documento de repaso. Para cada test: qué pide, qué SQL resuelve, qué concepto evalúan y la pregunta trampa asociada.

> **Regla de oro de toda la prueba:** el SQL que escribes está atado al C#. Los alias de columna (`AS`) deben coincidir **exactamente** (mayúsculas incluidas) con la cadena de `x.Field<T>("...")`, y el tipo `<T>` debe casar con el tipo real de la columna. El dev ejecuta después: si el nombre o el tipo no coinciden, revienta en runtime.

---

## 1. `CountTripsType` — Cuántos viajes hay de cada tipo

**Fin:** contar, agrupando por tipo, cuántos viajes existen de cada categoría (Beach, City, Adventure...).

```sql
SELECT Type, COUNT(*) AS TripsTotal
FROM Trips
GROUP BY Type
```

**Concepto evaluado:** agregación con `GROUP BY` + `COUNT`.

- `GROUP BY Type` colapsa todas las filas del mismo tipo en un solo grupo.
- `COUNT(*)` cuenta las filas de cada grupo.
- El alias `AS TripsTotal` es **obligatorio**: sin él la columna agregada no tendría nombre y `Field<int>("TripsTotal")` no la encontraría.

**Regla clave:** toda columna del `SELECT` que no esté dentro de un agregado (aquí `Type`) **debe** aparecer en el `GROUP BY`. Si se olvida, SQL Server da error.

**Mapeo C#:** objeto `TripsByType { string Type; int TripsTotal; }`.

---

## 2. `DiferentTripTypes` — Tipos de viaje distintos, sin duplicar

**Fin:** obtener la lista de tipos únicos, sin repeticiones.

```sql
SELECT DISTINCT Type
FROM Trips
```

**Concepto evaluado:** deduplicación con `DISTINCT`.

- `DISTINCT` elimina filas repetidas del resultado.
- No hay `GROUP BY` porque no se agrega nada: solo se quiere la lista limpia.

**Pregunta trampa — `DISTINCT` vs `GROUP BY Type`:** aquí dan el mismo resultado. La diferencia es la **intención**: `GROUP BY` se usa cuando vas a **agregar** (contar, sumar) sobre cada grupo; `DISTINCT` cuando solo quieres **valores únicos** sin calcular nada. Deduplicar con `GROUP BY` sin agregado funciona pero comunica peor la intención.

**Mapeo C#:** directo a `string` (sin clase contenedora).

---

## 3. `TypesWithMoreThanNTrips` — Tipos con más de 5 viajes

**Fin:** como el caso 1, pero quedándose solo con los tipos que superan un umbral de viajes.

```sql
SELECT Type, COUNT(*) AS TripsTotal
FROM Trips
GROUP BY Type
HAVING COUNT(*) > 5
```

**Concepto evaluado (el estrella): `WHERE` vs `HAVING`.**

- `WHERE` filtra filas **antes** de agrupar.
- `HAVING` filtra grupos **después** de agrupar.
- Por eso `HAVING` puede usar `COUNT(*)` y `WHERE` no: cuando se evalúa el `WHERE`, los grupos todavía no existen.

**Pregunta trampa — "¿por qué no `WHERE COUNT(*) > 5`?":** porque en el momento del `WHERE` aún no se ha agrupado, así que `COUNT(*)` no tiene sentido todavía. El filtro sobre un agregado va siempre en `HAVING`.

**Mapeo C#:** reutiliza `TripsByType`.

---

## 4. `TripsCountByClient` — Cuántos viajes ha reservado cada cliente

**Fin:** cruzar viajes con clientes y contar cuántos viajes tiene cada uno, **incluyendo** a los clientes que no han reservado ninguno.

```sql
SELECT c.Name AS ClientName, COUNT(t.TripId) AS TripsTotal
FROM Clients c
LEFT JOIN Trips t ON c.ClientId = t.ClientId
GROUP BY c.Name
```

**Concepto evaluado:** `JOIN` + `GROUP BY`, y la diferencia entre tipos de JOIN.

- **Alias de tabla** (`Clients c`, `Trips t`) para abreviar.
- `LEFT JOIN`: devuelve **todos** los clientes; los que no tienen viajes salen con `TripsTotal = 0`.
- `INNER JOIN` (alternativa): solo clientes con al menos un viaje.
- `COUNT(t.TripId)` en vez de `COUNT(*)`: con `LEFT JOIN`, un cliente sin viajes genera una fila con la parte de `Trips` en NULL. `COUNT(*)` la contaría como 1; `COUNT(columna)` ignora los NULL y da 0 correctamente.

**Pregunta trampa — "¿y si dos clientes se llaman igual?":** agrupar por `c.Name` los mezclaría. Lo robusto es `GROUP BY c.ClientId, c.Name`.

**Mapeo C#:** objeto `TripsByClient { string ClientName; int TripsTotal; }`.

---

## 5. `TopExpensiveTrips` — Los 3 viajes más caros

**Fin:** traer solo los 3 viajes de mayor precio, ordenados de más caro a más barato.

```sql
SELECT TOP 3 Destination, Price
FROM Trips
ORDER BY Price DESC
```

**Concepto evaluado:** limitar resultados en SQL Server + ordenación.

- En SQL Server es **`TOP n`**, no `LIMIT` (eso es MySQL/PostgreSQL). Decir `LIMIT` en una prueba de SQL Server es mala señal.
- `ORDER BY Price DESC` ordena descendente. `TOP` sin `ORDER BY` no tiene sentido: no sabrías "los más caros" respecto a qué orden.
- Para paginar, la forma de SQL Server es `OFFSET n ROWS FETCH NEXT m ROWS ONLY` (obliga a tener `ORDER BY`).

**Mapeo C#:** objeto `TripPrice { string Destination; decimal Price; }`. Ojo: `Price` es `decimal`, no `int`.

---

## Resumen de conceptos por test

| Test | Concepto central | Qué demuestra |
|------|------------------|---------------|
| `CountTripsType` | `GROUP BY` + `COUNT` | Agregación básica |
| `DiferentTripTypes` | `DISTINCT` | Deduplicación e intención vs `GROUP BY` |
| `TypesWithMoreThanNTrips` | `HAVING` | Filtrar grupos (`WHERE` vs `HAVING`) |
| `TripsCountByClient` | `LEFT JOIN` + `COUNT(col)` | JOINs y manejo de NULLs |
| `TopExpensiveTrips` | `TOP n` + `ORDER BY` | Específico de SQL Server |

---