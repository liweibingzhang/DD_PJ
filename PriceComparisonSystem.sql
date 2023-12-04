create database `PCS`;
drop database `PCS`;
use `PCS`;

-- 用户信息
create table `user`(
	`ID` int auto_increment,
    `user_name` varchar(20) not null unique,
	`password` varchar(20) not null,
    `fullname` varchar(40),
	`age` int,
	`birth_date` date,
	primary key(`ID`)
);
drop table `user`;

-- 商家信息
create table `seller`(
	`ID` int auto_increment,
    `seller_name` varchar(20) not null unique,
	`password` varchar(20) not null,
    `fullname` varchar(40),
    `address` varchar(50),
	`description` varchar(100),
	primary key(`ID`)
);
drop table `seller`;

-- 管理员信息
create table `administrator`(
	`ID` int auto_increment,
    `administrator_name` varchar(20) not null unique,
	`password` varchar(20) not null,
	primary key(`ID`)
);
drop table `administrator`;
-- 唯一的一位管理员
insert into `administrator` values(null, 'administrator', '123456');

-- 商品信息
create table `commodity`(
	`ID` int auto_increment,
	`commodity_name` varchar(20) not null unique,
	`category` varchar(10) not null,
	primary key(`ID`),
    check(`category` in ('食品类', '服装类', '鞋帽类', '日用品类', '家具类', '家用电器类', '五金类', '厨具类'))
);
drop table `commodity`;

-- 平台信息
create table `platform`(
	`ID` int auto_increment,
	`platform_name` varchar(20)  not null unique,
	primary key(`ID`)
);
drop table `platform`;

-- 历史价格
create table `history_price`(
	`commodity_id` int not null,
	`seller_id` int not null,
	`platform_id` int not null,
	`time` datetime not null,
	`price` decimal(7, 2) not null,
    primary key(`commodity_id`, `seller_id`, `platform_id`, `time`),
    foreign key(`commodity_id`) references `commodity`(`ID`) on delete cascade,
    foreign key(`seller_id`) references `seller`(`ID`) on delete cascade,
    foreign key(`platform_id`) references `platform`(`ID`) on delete cascade
);
drop table `history_price`;

-- 在售商品信息
create table `selling`(
	`commodity_id` int not null,
	`seller_id` int not null,
	`platform_id` int not null,
	`produce_date` date not null,
	`shelf_life` int not null,
	`produce_address` varchar(50) not null,
    `price` decimal(7, 2) not null,
	`description` varchar(100),
    primary key(`commodity_id`, `seller_id`, `platform_id`),
    foreign key(`commodity_id`) references `commodity`(`ID`) on delete cascade,
    foreign key(`seller_id`) references `seller`(`ID`) on delete cascade,
    foreign key(`platform_id`) references `platform`(`ID`) on delete cascade
);
drop table `selling`;

-- 用户收藏
create table `collects`(
	`user_id` int not null,
	`commodity_id` int not null,
	`seller_id` int not null,
	`platform_id` int not null,
	`price_floor` decimal(7, 2),
	primary key(`user_id`, `commodity_id`, `seller_id`, `platform_id`),
	foreign key(`user_id`) references `user`(`ID`) on delete cascade,
	foreign key(`commodity_id`) references `commodity`(`ID`) on delete cascade,
    foreign key(`seller_id`) references `seller`(`ID`) on delete cascade,
    foreign key(`platform_id`) references `platform`(`ID`) on delete cascade
);
drop table `collects`;

-- 用户收到的消息
create table `message`(
	`ID` int auto_increment,
    `user_id` int not null,
	`type` varchar(10) not null,
	`time` datetime not null,
	`state` char(2) not null,
	`content` varchar(255),
	primary key(`ID`),
    foreign key(`user_id`) references `user`(`ID`) on delete cascade,
    check(`type` in ('下架提醒', '有更低价')),
    check(`state` in ('已读', '未读'))
);
drop table `message`;


-- 触发器

-- 新发布在售商品后，在历史价格里插入一条数据
create trigger `tri_insert_selling` after insert
on `selling` for each row
insert into `history_price` values (new.commodity_id, new.seller_id, new.platform_id, now(), new.price);
drop trigger `tri_insert_selling`;

-- 更新了在售商品的价格后，在历史价格里插入一条数据
delimiter //
create trigger `tri_update_selling`
after update on `selling`
for each row
BEGIN
	if new.price != old.price then
		insert into `history_price` values (new.commodity_id, new.seller_id, new.platform_id, now(), new.price);
	end if;
END
//delimiter ;
drop trigger `tri_update_selling`;

-- 当在售商品的价格变得低于用户收藏设置的价格下限时，给用户发一条'有更低价'消息
delimiter //
create trigger `tri_update_selling_lowerprice`
after update on `selling`
for each row
BEGIN
	if old.price != new.price then
		insert into `message`(`user_id`, `type`, `time`, `state`, `content`)
		select `user_id`, '有更低价', now(), '未读', concat('您收藏的商品', `commodity`.commodity_name, '在', `platform`.platform_name, '平台的商家', `seller`.seller_name, '处有更低价')
		from `collects` join `commodity` on `collects`.commodity_id = `commodity`.ID
		join `seller` on `collects`.seller_id = `seller`.ID
		join `platform` on `collects`.platform_id = `platform`.ID
		where `collects`.commodity_id = new.commodity_id and `price_floor` is not null and new.price <= `price_floor` and old.price > `price_floor`;
	end if;
END
//delimiter ;
drop trigger `tri_update_selling_lowerprice`;


-- 当在售商品下架时，给受到影响的用户发一条'下架提醒'
delimiter //
create trigger `tri_delete_selling`
after delete on `selling`
for each row
BEGIN
	insert into `message`(`user_id`, `type`, `time`, `state`, `content`)
		select `user_id`, '下架提醒', now(), '未读', concat('您收藏的商品', `commodity`.commodity_name, '已被下架')
		from `collects` join `commodity` on `collects`.commodity_id = `commodity`.ID
		where `collects`.commodity_id = old.commodity_id;
END
//delimiter ;
drop trigger `tri_delete_selling`;
