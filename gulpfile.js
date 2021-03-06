'use strict';

//includes
var gulp = require('gulp'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    cleancss = require('gulp-clean-css'),
    less = require('gulp-less'),
    rename = require('gulp-rename'),
    merge = require('merge-stream'),
    config = require('./App/config.json');
    
//get config variables from config.json
var environment = config.environment;

//determine environment
var prod = false;
if (environment != 'dev' && environment != 'development' && environment != null) {
    //using staging or production environment
    prod = true;
}

//paths
var paths = {
    scripts: './App/Scripts/',
    css: './App/CSS/',
    app: './App/',
    themes: './App/Content/themes/',
    vendor: {
        root: './App/Vendor/**/'
    },
    webroot: './App/wwwroot/',
};

//working paths
paths.working = {
    js: {
        platform: [
            paths.scripts + 'selector/selector.js',
            paths.scripts + 'utility/velocity.min.js',
            paths.scripts + 'platform/_super.js', // <---- Datasilk Core Js: S object
            paths.scripts + 'platform/ajax.js', //   <---- Optional platform features
            paths.scripts + 'platform/drag.js',
            paths.scripts + 'platform/loader.js',
            paths.scripts + 'platform/message.js',
            //paths.scripts + 'platform/polyfill.js', // <---- used for older web browsers
            paths.scripts + 'platform/popup.js',
            //paths.scripts + 'platform/view.js', // <---- client-side mustache parser
            paths.scripts + 'platform/svg.js',
            paths.scripts + 'platform/util.js',
            //paths.scripts + 'platform/util.color.js',
            //paths.scripts + 'platform/util.file.js',
            paths.scripts + 'platform/validate.js',
            paths.scripts + 'platform/window.js' //  <---- End of Optional features
        ],
        app: [paths.app + '**/*.js'],
        utility: [
            paths.scripts + 'utility/*.js',
            paths.scripts + 'utility/**/*.js'
        ]
    },

    less:{
        platform: paths.css + 'platform.less',
        app: [
            paths.app + '**/*.less'
        ],
        themes: paths.css + 'themes/*.less',
        tapestry: paths.css + 'tapestry/tapestry.less',
        utility: paths.css + 'utility/*.less'
    },

    css: {
        utility: paths.css + 'utility/**/*.css',
        themes: paths.themes + '**/*.css',
        app: [paths.app + '**/*.css']
    },

    exclude: {
        app: [
            '!' + paths.app + 'wwwroot/**/*',
            '!' + paths.app + 'Content/**/*',
            '!' + paths.app + 'CSS/**/*',
            '!' + paths.app + 'CSS/*',
            '!' + paths.app + 'Scripts/**/*',
            '!' + paths.app + 'obj/**/*',
            '!' + paths.app + 'bin/**/*',
            '!' + paths.app + '**/node_modules/*'
        ]
    }
};

//compiled paths
paths.compiled = {
    platform: paths.webroot + 'js/platform.js',
    js: paths.webroot + 'js/',
    css: paths.webroot + 'css/',
    app: paths.webroot + 'css/',
    themes: paths.webroot + 'css/themes/'
};

//tasks for compiling javascript //////////////////////////////////////////////////////////////
gulp.task('js:app', function () {
    var pathlist = [...paths.working.js.app, ...paths.working.exclude.app];
    var p = gulp.src(pathlist)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    return p.pipe(gulp.dest(paths.compiled.js, { overwrite: true }));
});

gulp.task('js:platform', function () {
    var p = gulp.src(paths.working.js.platform, { base: '.' })
        .pipe(concat(paths.compiled.platform));
    return p.pipe(gulp.dest('.', { overwrite: true }));
});

gulp.task('js:utility', function () {
    var p = gulp.src(paths.working.js.utility)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    return p.pipe(gulp.dest(paths.compiled.js + 'utility', { overwrite: true }));
});

gulp.task('js', gulp.series('js:app', 'js:platform', 'js:utility'));

//tasks for compiling LESS & CSS /////////////////////////////////////////////////////////////////////
gulp.task('less:app', function () {
    var pathlist = [...paths.working.less.app, ...paths.working.exclude.app];
    var p = gulp.src(pathlist)
        .pipe(less())
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    //if(prod == true){ p = p.pipe(cleancss()); }
    return p.pipe(gulp.dest(paths.compiled.app, { overwrite: true }));
});

gulp.task('less:platform', function () {
    var p = gulp.src(paths.working.less.platform)
        .pipe(less());
    //if (prod == true) { p = p.pipe(cleancss()); }
    return p.pipe(gulp.dest(paths.compiled.css, { overwrite: true }));
});

gulp.task('less:themes', function () {
    var p = gulp.src(paths.working.less.themes)
        .pipe(less());
    //if (prod == true) { p = p.pipe(cleancss()); }
    return p.pipe(gulp.dest(paths.compiled.css + 'themes', { overwrite: true }));
});

gulp.task('less:utility', function () {
    var p = gulp.src(paths.working.less.utility)
        .pipe(less());
    //if (prod == true) { p = p.pipe(cleancss()); }
    return p.pipe(gulp.dest(paths.compiled.css + 'themes', { overwrite: true }));
});

gulp.task('css:themes', function () {
    var p = gulp.src(paths.working.css.themes)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    //if (prod == true) { p = p.pipe(cleancss()); }
    return p.pipe(gulp.dest(paths.compiled.themes, { overwrite: true }));
});

gulp.task('css:app', function () {
    var pathlist = [...paths.working.css.app, ...paths.working.exclude.app];
    var p = gulp.src(pathlist)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    //if (prod == true) { p = p.pipe(cleancss()); }
    return p.pipe(gulp.dest(paths.compiled.app, { overwrite: true }));
});

gulp.task('css:utility', function () {
    var p = gulp.src(paths.working.css.utility)
        .pipe(rename(function (path) {
            path.dirname = path.dirname.toLowerCase();
            path.basename = path.basename.toLowerCase();
            path.extname = path.extname.toLowerCase();
        }));
    //if (prod == true) { p = p.pipe(cleancss()); }
    return p.pipe(gulp.dest(paths.compiled.css + 'utility', { overwrite: true }));
});



gulp.task('less', gulp.series('less:platform', 'less:app', 'less:themes', 'less:utility'));

gulp.task('css', gulp.series('css:themes', 'css:app', 'css:utility')); 

//default task
gulp.task('default', gulp.series('js', 'less', 'css'));

//watch task
gulp.task('watch', function () {
    //watch platform JS
    gulp.watch(paths.working.js.platform, gulp.series('js:platform'));

    //watch app JS
    var pathjs = [...paths.working.js.app, ...paths.working.exclude.app.map(a => a + '*.js')];
    gulp.watch(pathjs, gulp.series('js:app'));

    //watch app LESS
    var pathless = [...paths.working.less.app, ...paths.working.exclude.app.map(a => a + '*.less')];
    gulp.watch(pathless, gulp.series('less:app'));

    //watch platform LESS
    gulp.watch([
        ...paths.working.less.platform,
        ...paths.working.less.tapestry
    ], gulp.series('less:platform'));

    //watch themes LESS
    gulp.watch([
        paths.working.less.themes
    ], gulp.series('less:themes', 'less:platform'));

    //watch utility LESS
    gulp.watch([
        paths.working.less.utility
    ], ['less:utility']);

    //watch app CSS
    var pathcss = [...paths.working.css.app, ...paths.working.exclude.app.map(a => a + '*.css')];
    gulp.watch(pathcss, gulp.series('css:app'));

    //watch themes CSS
    gulp.watch([paths.working.css.themes], gulp.series('css:themes'));

    //watch utility CSS
    gulp.watch([paths.working.css.utility], gulp.series('css:utility'));
});