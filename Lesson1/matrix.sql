SELECT
    c.id AS consumer_id,
    c.name AS consumer_name,
    c2.id as producer_id,
    c2.name as producer_name,
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
    c.id, c2.id;