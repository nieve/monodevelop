
EXTRA_DIST = configure

all: update_submodules all-recursive

update_submodules:
	git submodule update --init --recursive

top_srcdir=.
include $(top_srcdir)/config.make

CONFIG_MAKE=$(top_srcdir)/config.make

%-recursive: $(CONFIG_MAKE)
	@export PKG_CONFIG_PATH="`pwd`/$(top_srcdir)/local-config:$(prefix)/lib/pkgconfig:$(prefix)/share/pkgconfig:$$PKG_CONFIG_PATH"; \
	export MONO_GAC_PREFIX="$(prefix):$$MONO_GAC_PREFIX"; \
	set . $$MAKEFLAGS; final_exit=:; \
	case $$2 in --unix) shift ;; esac; \
	case $$2 in *=*) dk="exit 1" ;; *k*) dk=: ;; *) dk="exit 1" ;; esac; \
	for dir in $(SUBDIRS); do \
		case $$dir in \
		.) make $*-local || { final_exit="exit 1"; $$dk; };;\
		*) (cd $$dir && make $*) || { final_exit="exit 1"; $$dk; };;\
		esac \
	done
	$$final_exit

$(CONFIG_MAKE): $(top_srcdir)/configure
	@if test -e "$(CONFIG_MAKE)"; then exec $(top_srcdir)/configure --prefix=$(prefix); \
	else \
		echo "You must run configure first"; \
		exit 1; \
	fi

clean: clean-recursive
install: install-recursive
uninstall: uninstall-recursive
distcheck: distcheck-recursive

distclean: distclean-recursive
	rm -rf config.make local-config

dist: dist-recursive
	rm -rf tarballs
	mkdir -p tarballs
	for t in $(SUBDIRS); do \
		if test -a $$t/*.tar.gz; then \
			mv -f $$t/*.tar.gz tarballs ;\
		fi \
	done
	for t in `ls tarballs/*.tar.gz`; do \
		gunzip $$t ;\
	done
	for t in `ls tarballs/*.tar`; do \
		bzip2 $$t ;\
	done
	rm -rf specs
	mkdir -p specs
	for t in $(SUBDIRS); do \
		if test -a $$t/*.spec; then \
			cp -f $$t/*.spec specs ;\
		fi \
	done

run:
	cd main && make run

run-gdb:
	cd main && make run-gdb

test:
	cd main/tests/UnitTests && make test fixture=$(fixture)

check-addins:
	cd main && make check-addins

app-dir:
	cd main && make app-dir

package-monomac:
	(cd main; make package-monomac)

reset-versions: reset-all
check-versions: check-all

reset-%:
	@./version-checks --reset $*

check-%:
	@./version-checks --check $*
