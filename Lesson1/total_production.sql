WITH total_consumer_consumption AS (
    SELECT
        c.id AS company_id,
        COALESCE(SUM(crb.count_product * p.price), 0) as total_consumed
    FROM
        product p
    RIGHT JOIN
        company c on p.company_id = c.id
    LEFT JOIN
        consumer_requirement_base crb ON p.id = crb.product_id
    GROUP BY
        c.id, c.name
    ORDER BY
        c.id
),
total_manufacturer_consumption AS (
    SELECT
        producer_id as company_id,
        SUM(m.value) as value
    FROM
        (SELECT
            c.id AS consumer_id,
            c2.id as producer_id,
            SUM(COALESCE(cr.count_product * p.price, 0)) AS value
        FROM
            company c
        CROSS JOIN
            company c2
        LEFT JOIN
            product p ON c2.id = p.company_id
        LEFT JOIN
            company_requirement cr ON c.id = cr.company_id AND p.id = cr.product_id
        GROUP BY
            c.id, c.name, c2.id, c2.name
        ORDER BY
            c.id, c2.id) m
    GROUP BY
        m.producer_id
)
SELECT
    tmc.company_id as company_id,
    tmc.value + tcc.total_consumed as value
FROM
    total_consumer_consumption tcc
JOIN
    total_manufacturer_consumption tmc ON tcc.company_id = tmc.company_id
ORDER BY
    company_id