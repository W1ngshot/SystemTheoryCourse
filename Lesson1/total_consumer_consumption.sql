SELECT
    c.id AS company_id,
    --c.name AS company_name,
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
    c.id;