DO
$$
    DECLARE
        first_names         TEXT[] := ARRAY ['John', 'Jane', 'Alice', 'Bob', 'Charlie', 'Dave', 'Eve', 'Faythe', 'Grace', 'Heidi'];
        last_names          TEXT[] := ARRAY ['Doe', 'Smith', 'Johnson', 'Williams', 'Brown', 'Jones', 'Garcia', 'Miller', 'Davis', 'Rodriguez'];
        genders             TEXT[] := ARRAY [
            'Агендер', 'Андрогін', 'Бігендер', 'Цисгендер', 'Цисгендерна небінарна особа',
            'Демігендер', 'Джендерквір', 'Гендерфлюїд', 'Гендернеконформна особа',
            'Інтерсексуальна особа', 'Нонбінарна особа', 'Пангендер', 'Полігендер',
            'Трансгендерна жінка', 'Трансгендерний чоловік'
            ];
        sexes               TEXT[] := ARRAY ['Чоловік', 'Жінка', 'Інтерсекс'];
        sexual_orientations TEXT[] := ARRAY [
            'Гетеросексуальність', 'Гомосексуальність', 'Бісексуальність', 'Пансексуальність',
            'Асексуальність', 'Демісексуальність', 'Полісексуальність', 'Квір',
            'Грейсексуальність', 'Автосексуальність', 'Сколіосексуальність', 'Андросексуальність',
            'Гінекосексуальність'
            ];
        cities              TEXT[] := ARRAY ['Київ','Львів','Одеса','Дніпро','Харків','Запоріжжя','Луцьк','Чернівці','Івано-Франківськ',
            'Ужгород','Рівне','Тернопіль','Вінниця','Полтава', 'Суми','Херсон','Черкаси','Чернігів','Миколаїв','Кривий Ріг'];
        zodiac_signs        TEXT[] := ARRAY ['Овен', 'Телець', 'Близнюки', 'Рак', 'Лев', 'Діва', 'Терези', 'Скорпіон', 'Стрілець', 'Козеріг', 'Водолій', 'Риби'];
        i                   INT;
        random_date         DATE;
    BEGIN
        FOR i IN 1..500
            LOOP
                random_date := date '2023-01-01' + ((random() * (date '2024-07-01' - date '2023-01-01'))::INT);
                INSERT INTO clients (first_name, last_name, gender, sex, sexual_orientation, location,
                                     registration_number,
                                     registered_on, age, height, weight, zodiac_sign, description)
                VALUES (first_names[ceil(random() * array_length(first_names, 1))],
                        last_names[ceil(random() * array_length(last_names, 1))],
                        genders[ceil(random() * array_length(genders, 1))],
                        sexes[ceil(random() * array_length(sexes, 1))],
                        sexual_orientations[ceil(random() * array_length(sexual_orientations, 1))],
                        cities[ceil(random() * array_length(cities, 1))],
                        'REG' || lpad(i::text, 3, '0'),
                        random_date,
                        (random() * 52 + 18)::INT,
                        round((random() * 50 + 150)::NUMERIC, 1),
                        round((random() * 50 + 50)::NUMERIC, 2),
                        zodiac_signs[ceil(random() * array_length(zodiac_signs, 1))],
                        'Description ' || i);
            END LOOP;
    END
$$;

INSERT INTO Invitations (inviter_id, invitee_id, location, date_of_meeting, created_on, active_to, is_accepted)
VALUES (1, 2, 'Central Park', '2024-08-10 10:00:00', '2024-08-01', '2024-08-31', true),
       (2, 3, 'Times Square', '2024-08-11 11:00:00', '2024-08-02', '2024-09-01', false),
       (3, 4, 'Statue of Liberty', '2024-08-12 12:00:00', '2024-08-03', '2024-09-02', true),
       (4, 5, 'Brooklyn Bridge', '2024-08-13 13:00:00', '2024-08-04', '2024-09-03', false),
       (5, 6, 'Empire State Building', '2024-08-14 14:00:00', '2024-08-05', '2024-09-04', true),
       (6, 7, 'Rockefeller Center', '2024-08-15 15:00:00', '2024-08-06', '2024-09-05', false),
       (7, 8, 'Broadway', '2024-08-16 16:00:00', '2024-08-07', '2024-09-06', true),
       (8, 9, 'Wall Street', '2024-08-17 17:00:00', '2024-08-08', '2024-09-07', false),
       (9, 10, 'Fifth Avenue', '2024-08-18 18:00:00', '2024-08-09', '2024-09-08', true),
       (10, 1, 'Metropolitan Museum', '2024-08-19 19:00:00', '2024-08-10', '2024-09-09', false),
       (1, 3, 'Central Park', '2024-08-20 10:00:00', '2024-08-11', '2024-09-10', true),
       (2, 4, 'Times Square', '2024-08-21 11:00:00', '2024-08-12', '2024-09-11', false),
       (3, 5, 'Statue of Liberty', '2024-08-22 12:00:00', '2024-08-13', '2024-09-12', true),
       (4, 6, 'Brooklyn Bridge', '2024-08-23 13:00:00', '2024-08-14', '2024-09-13', false),
       (5, 7, 'Empire State Building', '2024-08-24 14:00:00', '2024-08-15', '2024-09-14', true),
       (6, 8, 'Rockefeller Center', '2024-08-25 15:00:00', '2024-08-16', '2024-09-15', false),
       (7, 9, 'Broadway', '2024-08-26 16:00:00', '2024-08-17', '2024-09-16', true),
       (8, 10, 'Wall Street', '2024-08-27 17:00:00', '2024-08-18', '2024-09-17', false),
       (9, 1, 'Fifth Avenue', '2024-08-28 18:00:00', '2024-08-19', '2024-09-18', true),
       (10, 2, 'Metropolitan Museum', '2024-08-29 19:00:00', '2024-08-20', '2024-09-19', false),
       (1, 4, 'Central Park', '2024-08-30 10:00:00', '2024-08-21', '2024-09-20', true),
       (2, 5, 'Times Square', '2024-08-31 11:00:00', '2024-08-22', '2024-09-21', false),
       (3, 6, 'Statue of Liberty', '2024-09-01 12:00:00', '2024-08-23', '2024-09-22', true),
       (4, 7, 'Brooklyn Bridge', '2024-09-02 13:00:00', '2024-08-24', '2024-09-23', false),
       (5, 8, 'Empire State Building', '2024-09-03 14:00:00', '2024-08-25', '2024-09-24', true),
       (6, 9, 'Rockefeller Center', '2024-09-04 15:00:00', '2024-08-26', '2024-09-25', false),
       (7, 10, 'Broadway', '2024-09-05 16:00:00', '2024-08-27', '2024-09-26', true),
       (8, 1, 'Wall Street', '2024-09-06 17:00:00', '2024-08-28', '2024-09-27', false),
       (9, 2, 'Fifth Avenue', '2024-09-07 18:00:00', '2024-08-29', '2024-09-28', true),
       (10, 3, 'Metropolitan Museum', '2024-09-08 19:00:00', '2024-08-30', '2024-09-29', false),
       (1, 5, 'Central Park', '2024-09-09 10:00:00', '2024-08-31', '2024-09-30', true),
       (2, 6, 'Times Square', '2024-09-10 11:00:00', '2024-09-01', '2024-10-01', false),
       (3, 7, 'Statue of Liberty', '2024-09-11 12:00:00', '2024-09-02', '2024-10-02', true),
       (4, 8, 'Brooklyn Bridge', '2024-09-12 13:00:00', '2024-09-03', '2024-10-03', false),
       (5, 9, 'Empire State Building', '2024-09-13 14:00:00', '2024-09-04', '2024-10-04', true),
       (6, 10, 'Rockefeller Center', '2024-09-14 15:00:00', '2024-09-05', '2024-10-05', false),
       (7, 1, 'Broadway', '2024-09-15 16:00:00', '2024-09-06', '2024-10-06', true),
       (8, 2, 'Wall Street', '2024-09-16 17:00:00', '2024-09-07', '2024-10-07', false),
       (9, 3, 'Fifth Avenue', '2024-09-17 18:00:00', '2024-09-08', '2024-10-08', true),
       (10, 4, 'Metropolitan Museum', '2024-09-18 19:00:00', '2024-09-09', '2024-10-09', false),
       (1, 6, 'Central Park', '2024-09-19 10:00:00', '2024-09-10', '2024-10-10', true),
       (2, 7, 'Times Square', '2024-09-20 11:00:00', '2024-09-11', '2024-10-11', false),
       (3, 8, 'Statue of Liberty', '2024-09-21 12:00:00', '2024-09-12', '2024-10-12', true),
       (4, 9, 'Brooklyn Bridge', '2024-09-22 13:00:00', '2024-09-13', '2024-10-13', false),
       (5, 10, 'Empire State Building', '2024-09-23 14:00:00', '2024-09-14', '2024-10-14', true),
       (6, 1, 'Rockefeller Center', '2024-09-24 15:00:00', '2024-09-15', '2024-10-15', false),
       (7, 2, 'Broadway', '2024-09-25 16:00:00', '2024-09-16', '2024-10-16', true),
       (8, 3, 'Wall Street', '2024-09-26 17:00:00', '2024-09-17', '2024-10-17', false),
       (9, 4, 'Fifth Avenue', '2024-09-27 18:00:00', '2024-09-18', '2024-10-18', true),
       (10, 5, 'Metropolitan Museum', '2024-09-28 19:00:00', '2024-09-19', '2024-10-19', false);


